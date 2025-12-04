namespace HCIExplorer.Models;

public enum LogEntryType
{
    Request,
    Response,
    Debug,
    Error
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogEntryType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? HexData { get; set; }
    
    public LogEntry(LogEntryType type, string message, string? hexData = null)
    {
        Timestamp = DateTime.Now;
        Type = type;
        Message = message;
        HexData = hexData;
    }
}
