using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace RESTLibrary;

/// <summary>
/// Represents a device discovered via mDNS.
/// </summary>
public class DiscoveredDevice
{
    /// <summary>Hostname from the mDNS response (e.g. "CC-ARC-012345").</summary>
    public string Hostname { get; set; } = string.Empty;

    /// <summary>IP address of the device.</summary>
    public IPAddress? Address { get; set; }

    /// <summary>Port number advertised by the service (typically 443 or 80).</summary>
    public int Port { get; set; }

    /// <summary>The service type that matched (e.g. "_https._tcp.local.").</summary>
    public string ServiceType { get; set; } = string.Empty;

    /// <summary>Whether this appears to be an Arcadia device (hostname starts with CC-ARC).</summary>
    public bool IsArcadia => Hostname.StartsWith("CC-ARC", StringComparison.OrdinalIgnoreCase);

    /// <summary>Whether this appears to be an LQ device (hostname starts with CC-LQ).</summary>
    public bool IsLQ => Hostname.StartsWith("CC-LQ", StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override string ToString()
    {
        string type = IsArcadia ? "Arcadia" : IsLQ ? "LQ" : "Unknown";
        return $"{type}: {Hostname} @ {Address}:{Port}";
    }
}

/// <summary>
/// Simple mDNS (Multicast DNS) discovery for Clear-Com LQ and Arcadia devices.
/// Sends DNS-SD PTR queries for _https._tcp.local and _http._tcp.local,
/// then filters results by hostname prefix (CC-ARC for Arcadia, CC-LQ for LQ).
/// No external dependencies required.
/// </summary>
public static class DeviceDiscovery
{
    private static readonly IPAddress MdnsMulticastAddress = IPAddress.Parse("224.0.0.251");
    private const int MdnsPort = 5353;

    /// <summary>
    /// Discovers all Clear-Com devices (Arcadia and LQ) on the local network via mDNS.
    /// </summary>
    /// <param name="timeout">How long to listen for responses. Default 3 seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of discovered devices.</returns>
    public static async Task<List<DiscoveredDevice>> DiscoverAllAsync(
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(3);

        var serviceTypes = new[] { "_https._tcp.local.", "_http._tcp.local." };
        var devices = new Dictionary<string, DiscoveredDevice>(StringComparer.OrdinalIgnoreCase);

        foreach (var serviceType in serviceTypes)
        {
            var found = await QueryMdnsAsync(serviceType, timeout.Value, cancellationToken);
            foreach (var device in found)
            {
                if (device.IsArcadia || device.IsLQ)
                {
                    string key = $"{device.Hostname}:{device.Port}";
                    devices.TryAdd(key, device);
                }
            }
        }

        return devices.Values.ToList();
    }

    /// <summary>
    /// Discovers only Arcadia devices on the local network via mDNS.
    /// </summary>
    /// <param name="timeout">How long to listen for responses. Default 3 seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of discovered Arcadia devices.</returns>
    public static async Task<List<DiscoveredDevice>> DiscoverArcadiaAsync(
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var all = await DiscoverAllAsync(timeout, cancellationToken);
        return all.Where(d => d.IsArcadia).ToList();
    }

    /// <summary>
    /// Discovers only LQ devices on the local network via mDNS.
    /// </summary>
    /// <param name="timeout">How long to listen for responses. Default 3 seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of discovered LQ devices.</returns>
    public static async Task<List<DiscoveredDevice>> DiscoverLQAsync(
        TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var all = await DiscoverAllAsync(timeout, cancellationToken);
        return all.Where(d => d.IsLQ).ToList();
    }

    /// <summary>
    /// Sends an mDNS PTR query for the given service type and collects responses.
    /// Tries each available IPv4 network interface to maximise discovery chances.
    /// </summary>
    private static async Task<List<DiscoveredDevice>> QueryMdnsAsync(
        string serviceType, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var devices = new List<DiscoveredDevice>();
        byte[] query = BuildPtrQuery(serviceType);

        // Gather usable IPv4 addresses from all Up interfaces
        var localAddresses = GetUsableIPv4Addresses();
        if (localAddresses.Count == 0)
            localAddresses.Add(IPAddress.Any); // fallback

        foreach (var localAddress in localAddresses)
        {
            try
            {
                var found = await QueryMdnsOnInterfaceAsync(
                    query, serviceType, localAddress, timeout, cancellationToken);
                devices.AddRange(found);
            }
            catch (SocketException)
            {
                // Interface doesn't support multicast – skip
            }
        }

        return devices;
    }

    /// <summary>
    /// Returns a list of usable IPv4 unicast addresses from all operational network interfaces.
    /// </summary>
    private static List<IPAddress> GetUsableIPv4Addresses()
    {
        var addresses = new List<IPAddress>();
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up)
                    continue;
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;
                if (!nic.SupportsMulticast)
                    continue;

                var props = nic.GetIPProperties();
                foreach (var addr in props.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        addresses.Add(addr.Address);
                    }
                }
            }
        }
        catch
        {
            // If enumeration fails, caller will fall back to IPAddress.Any
        }
        return addresses;
    }

    /// <summary>
    /// Sends an mDNS query on a specific local interface and collects responses.
    /// </summary>
    private static async Task<List<DiscoveredDevice>> QueryMdnsOnInterfaceAsync(
        byte[] query, string serviceType, IPAddress localAddress,
        TimeSpan timeout, CancellationToken cancellationToken)
    {
        var devices = new List<DiscoveredDevice>();

        using var udp = new UdpClient(AddressFamily.InterNetwork);
        udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udp.ExclusiveAddressUse = false;
        udp.Client.Bind(new IPEndPoint(localAddress, MdnsPort));
        udp.MulticastLoopback = false;
        udp.Client.SetSocketOption(
            SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
        udp.JoinMulticastGroup(MdnsMulticastAddress, localAddress);

        await udp.SendAsync(query, query.Length, new IPEndPoint(MdnsMulticastAddress, MdnsPort));

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                var result = await udp.ReceiveAsync(cts.Token);
                var parsed = ParseMdnsResponse(result.Buffer, serviceType);
                if (parsed != null)
                {
                    parsed.Address ??= result.RemoteEndPoint.Address;
                    devices.Add(parsed);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException)
            {
                break;
            }
        }

        return devices;
    }

    /// <summary>
    /// Builds a minimal DNS query packet for a PTR record of the given service type.
    /// </summary>
    private static byte[] BuildPtrQuery(string serviceName)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // Header
        writer.Write((ushort)0);                 // Transaction ID
        writer.Write((byte)0); writer.Write((byte)0); // Flags: standard query
        writer.Write((byte)0); writer.Write((byte)1); // Questions: 1
        writer.Write((byte)0); writer.Write((byte)0); // Answer RRs
        writer.Write((byte)0); writer.Write((byte)0); // Authority RRs
        writer.Write((byte)0); writer.Write((byte)0); // Additional RRs

        // Question: encode the service name as DNS labels
        foreach (var label in serviceName.TrimEnd('.').Split('.'))
        {
            byte[] labelBytes = Encoding.UTF8.GetBytes(label);
            writer.Write((byte)labelBytes.Length);
            writer.Write(labelBytes);
        }
        writer.Write((byte)0); // Root label

        writer.Write((byte)0); writer.Write((byte)12); // QTYPE: PTR (12)
        writer.Write((byte)0); writer.Write((byte)1);  // QCLASS: IN (1)

        return ms.ToArray();
    }

    /// <summary>
    /// Parses an mDNS response packet, extracting hostname, IP, and port
    /// from PTR, SRV, and A records.
    /// </summary>
    private static DiscoveredDevice? ParseMdnsResponse(byte[] data, string serviceType)
    {
        try
        {
            if (data.Length < 12)
                return null;

            // Read header counts
            int answerCount = (data[6] << 8) | data[7];
            int authorityCount = (data[8] << 8) | data[9];
            int additionalCount = (data[10] << 8) | data[11];
            int totalRecords = answerCount + authorityCount + additionalCount;

            if (totalRecords == 0)
                return null;

            // Skip the question section
            int offset = 12;
            int questionCount = (data[4] << 8) | data[5];
            for (int i = 0; i < questionCount; i++)
            {
                offset = SkipDnsName(data, offset);
                offset += 4; // QTYPE + QCLASS
            }

            string? hostname = null;
            IPAddress? address = null;
            int port = 0;

            // Parse all resource records (answer + authority + additional)
            for (int i = 0; i < totalRecords && offset < data.Length; i++)
            {
                int nameEnd = SkipDnsName(data, offset);
                if (nameEnd + 10 > data.Length)
                    break;

                int rrOffset = nameEnd;
                ushort rrType = (ushort)((data[rrOffset] << 8) | data[rrOffset + 1]);
                // ushort rrClass at rrOffset + 2
                // uint ttl at rrOffset + 4
                ushort rdLength = (ushort)((data[rrOffset + 8] << 8) | data[rrOffset + 9]);
                int rdStart = rrOffset + 10;

                if (rdStart + rdLength > data.Length)
                    break;

                switch (rrType)
                {
                    case 12: // PTR
                        string ptrName = ReadDnsName(data, rdStart);
                        if (hostname == null)
                        {
                            // PTR value is typically "instancename._https._tcp.local."
                            // Extract the instance name (first label)
                            int dot = ptrName.IndexOf('.');
                            hostname = dot > 0 ? ptrName[..dot] : ptrName;
                        }
                        break;

                    case 33: // SRV
                        if (rdLength >= 6)
                        {
                            // Priority(2) + Weight(2) + Port(2) + Target
                            port = (data[rdStart + 4] << 8) | data[rdStart + 5];
                            string target = ReadDnsName(data, rdStart + 6);
                            // SRV target is the actual hostname (e.g. "CC-ARC-012345.local.")
                            int targetDot = target.IndexOf('.');
                            if (targetDot > 0)
                            {
                                hostname = target[..targetDot];
                            }
                            else if (!string.IsNullOrEmpty(target))
                            {
                                hostname = target;
                            }
                        }
                        break;

                    case 1: // A
                        if (rdLength == 4)
                        {
                            address = new IPAddress(new ReadOnlySpan<byte>(data, rdStart, 4));
                        }
                        break;
                }

                offset = rdStart + rdLength;
            }

            if (hostname == null)
                return null;

            return new DiscoveredDevice
            {
                Hostname = hostname,
                Address = address,
                Port = port > 0 ? port : 443,
                ServiceType = serviceType,
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Reads a DNS-encoded domain name handling compression pointers.
    /// </summary>
    private static string ReadDnsName(byte[] data, int offset)
    {
        var labels = new List<string>();
        int maxJumps = 64; // safety limit
        bool jumped = false;

        while (offset < data.Length && maxJumps-- > 0)
        {
            byte len = data[offset];
            if (len == 0)
            {
                break;
            }

            // Compression pointer (top 2 bits set)
            if ((len & 0xC0) == 0xC0)
            {
                if (offset + 1 >= data.Length)
                    break;
                int pointer = ((len & 0x3F) << 8) | data[offset + 1];
                offset = pointer;
                jumped = true;
                continue;
            }

            offset++;
            if (offset + len > data.Length)
                break;

            labels.Add(Encoding.UTF8.GetString(data, offset, len));
            offset += len;
        }

        return string.Join(".", labels);
    }

    /// <summary>
    /// Skips over a DNS-encoded name in the packet, returning the offset after the name.
    /// </summary>
    private static int SkipDnsName(byte[] data, int offset)
    {
        int maxJumps = 64;

        while (offset < data.Length && maxJumps-- > 0)
        {
            byte len = data[offset];
            if (len == 0)
            {
                return offset + 1;
            }

            // Compression pointer
            if ((len & 0xC0) == 0xC0)
            {
                return offset + 2;
            }

            offset += 1 + len;
        }

        return offset;
    }
}
