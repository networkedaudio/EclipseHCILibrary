using System.Net;
using System.Text;

namespace HCILibrary.Discovery;

/// <summary>
/// Parses UDP broadcast messages received on port 42001
/// </summary>
public static class BroadcastParser
{
    /// <summary>
    /// Parses raw broadcast data into structured format
    /// </summary>
    /// <param name="data">Raw byte array from UDP broadcast</param>
    /// <param name="source">Source IP endpoint</param>
    /// <returns>Parsed broadcast data or null if parsing fails</returns>
    public static BroadcastData? Parse(byte[] data, IPEndPoint source)
    {
        if (data == null || data.Length == 0)
        {
            return null;
        }

        try
        {
            var broadcastData = new BroadcastData
            {
                SourceAddress = source.Address.ToString(),
                SourcePort = source.Port,
                RawData = data,
                DataLength = data.Length,
                ReceivedTimestamp = DateTime.Now
            };

            // Try to decode as Eclipse HX matrix signature
            var signature = SignatureDecoder.GetSignature(data, source.Address);
            if (signature != null)
            {
                broadcastData.MessageType = "Eclipse HX";
                broadcastData.ParsedContent = FormatSignature(signature);
                broadcastData.MatrixSignature = signature;
            }
            // Try to parse as ASCII string
            else if (IsAsciiData(data))
            {
                broadcastData.MessageType = "ASCII";
                broadcastData.ParsedContent = Encoding.ASCII.GetString(data);
            }
            else
            {
                // Binary data - convert to hex string
                broadcastData.MessageType = "Binary";
                broadcastData.ParsedContent = BitConverter.ToString(data).Replace("-", " ");
            }

            return broadcastData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing broadcast: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Formats a matrix signature into a readable string
    /// </summary>
    private static string FormatSignature(MatrixSignature signature)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Eclipse HX Matrix:");
        sb.AppendLine($"  Frame Name: {signature.FrameName}");
        sb.AppendLine($"  Identity: {signature.Identity}");
        sb.AppendLine($"  Type: {signature.MatrixType}");
        sb.AppendLine($"  Version: {signature.MapVersion}");
        sb.AppendLine($"  System Number: {signature.SystemNumber}");
        if (signature.Created != default)
        {
            sb.AppendLine($"  Created: {signature.Created:yyyy-MM-dd HH:mm:ss}");
        }
        sb.AppendLine($"  Address: {signature.PrimaryAddress}");
        return sb.ToString();
    }

    /// <summary>
    /// Checks if data appears to be ASCII text
    /// </summary>
    private static bool IsAsciiData(byte[] data)
    {
        foreach (byte b in data)
        {
            if (b < 32 || b > 126)
            {
                // Non-printable ASCII character (excluding common control chars)
                if (b != 9 && b != 10 && b != 13) // Tab, LF, CR
                {
                    return false;
                }
            }
        }
        return true;
    }
}

/// <summary>
/// Represents parsed broadcast data
/// </summary>
public class BroadcastData
{
    public string SourceAddress { get; set; } = string.Empty;
    public int SourcePort { get; set; }
    public byte[] RawData { get; set; } = Array.Empty<byte>();
    public int DataLength { get; set; }
    public DateTime ReceivedTimestamp { get; set; }
    public string MessageType { get; set; } = "Unknown";
    public string? ParsedContent { get; set; }
    public MatrixSignature? MatrixSignature { get; set; }

    /// <summary>
    /// Gets the raw data as a hex string
    /// </summary>
    public string GetHexString()
    {
        return BitConverter.ToString(RawData).Replace("-", " ");
    }

    public override string ToString()
    {
        return $"[{ReceivedTimestamp:HH:mm:ss.fff}] {SourceAddress}:{SourcePort} - {MessageType} ({DataLength} bytes)";
    }
}