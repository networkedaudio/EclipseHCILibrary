namespace HCILibrary.Discovery;

/// <summary>
/// Helper class for reading data from a byte array sequentially
/// </summary>
internal class ByteStream
{
    private readonly byte[] _data;
    private int _position;

    public ByteStream(byte[] data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _position = 0;
    }

    public int Remaining => _data.Length - _position;

    public byte ReadByte()
    {
        if (_position >= _data.Length)
            throw new InvalidOperationException("Attempted to read beyond end of stream");

        return _data[_position++];
    }

    public byte[] ReadBytes(int count)
    {
        if (_position + count > _data.Length)
            throw new InvalidOperationException("Attempted to read beyond end of stream");

        var result = new byte[count];
        Array.Copy(_data, _position, result, 0, count);
        _position += count;
        return result;
    }

    public ulong ReadUlong()
    {
        if (_position + 8 > _data.Length)
            throw new InvalidOperationException("Attempted to read beyond end of stream");

        ulong result = BitConverter.ToUInt64(_data, _position);
        _position += 8;
        return result;
    }

    public string ReadASCIIString(int length)
    {
        var bytes = ReadBytes(length);
        // Find null terminator if present
        int nullIndex = Array.IndexOf(bytes, (byte)0);
        if (nullIndex >= 0)
        {
            bytes = bytes.Take(nullIndex).ToArray();
        }
        return System.Text.Encoding.ASCII.GetString(bytes).TrimEnd('\0', ' ');
    }
}