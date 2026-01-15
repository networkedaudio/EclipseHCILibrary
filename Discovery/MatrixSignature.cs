using System.Net;

namespace HCILibrary.Discovery;

/// <summary>
/// Represents the signature information from an Eclipse HX matrix broadcast
/// </summary>
public class MatrixSignature
{
    private int _systemNumber;

    public MatrixSignature(int systemNumber)
    {
        _systemNumber = systemNumber;
    }

    public IPAddress? PrimaryAddress { get; set; }
    public DateTime Created { get; internal set; }
    public string Identity { get; internal set; } = string.Empty;
    public byte[] IdentityByteArrays { get; internal set; } = Array.Empty<byte>();
    public MatrixType MatrixType { get; internal set; }
    public string MapVersion { get; internal set; } = string.Empty;
    public string FrameName { get; internal set; } = string.Empty;
    public uint Checksum { get; internal set; }

    public int SystemNumber
    {
        get => _systemNumber;
        set => _systemNumber = value;
    }

    public override string ToString()
    {
        return $"{FrameName} ({Identity}) - {MatrixType} v{MapVersion} @ {PrimaryAddress}";
    }
}