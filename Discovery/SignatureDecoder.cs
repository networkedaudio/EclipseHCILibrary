using System.Globalization;
using System.Net;
using System.Text;

namespace HCILibrary.Discovery;

/// <summary>
/// Decodes Eclipse HX matrix signature broadcasts
/// </summary>
internal static class SignatureDecoder
{
    /// <summary>
    /// Decodes a matrix signature from UDP broadcast data
    /// </summary>
    /// <param name="bytesReceived">Raw broadcast bytes</param>
    /// <param name="address">Source IP address</param>
    /// <returns>Decoded matrix signature or null if decoding fails</returns>
    internal static MatrixSignature? GetSignature(byte[] bytesReceived, IPAddress address)
    {
        try
        {
            if (bytesReceived == null || bytesReceived.Length < 50)
                return null;

            var stream = new ByteStream(bytesReceived);

            stream.ReadBytes(14);

            var signature = new MatrixSignature(stream.ReadByte())
            {
                PrimaryAddress = address
            };

            byte yearPart = stream.ReadByte();
            byte month = stream.ReadByte();
            byte day = stream.ReadByte();
            byte hour = stream.ReadByte();
            byte minute = stream.ReadByte();
            byte second = stream.ReadByte();
            byte millisecond = stream.ReadByte();

            int year = 2000 + yearPart;
            try
            {
                if (month != byte.MaxValue)
                {
                    signature.Created = new DateTime(year, month, day, hour, minute, second, millisecond);
                }
            }
            catch (Exception)
            {
                // Invalid date - leave as default
            }

            var identityBytesArray = stream.ReadBytes(18);

            signature.Identity = Encoding.ASCII.GetString(identityBytesArray).TrimEnd('\0');
            signature.IdentityByteArrays = identityBytesArray;

            stream.ReadByte(); // flags
            byte matrixType = stream.ReadByte(); // type

            signature.MatrixType = matrixType switch
            {
                1 => MatrixType.EclipseOmega,
                2 => MatrixType.EclipsePico,
                3 => MatrixType.EclipseMedian,
                4 => MatrixType.EclipseDelta,
                _ => MatrixType.NoneSelected
            };

            byte majorVersion = stream.ReadByte();
            byte minorVersion = stream.ReadByte();
            byte buildVersion = stream.ReadByte();
            byte build1Version = stream.ReadByte();

            signature.MapVersion = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}",
                majorVersion, minorVersion, buildVersion, build1Version);

            signature.FrameName = stream.ReadASCIIString(20);

            if (stream.Remaining >= 8)
            {
                signature.Checksum = (uint)stream.ReadUlong();
            }

            return signature;
        }
        catch (Exception)
        {
            return null;
        }
    }
}