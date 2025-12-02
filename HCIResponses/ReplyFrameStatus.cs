using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Frame Status (HCIv2) - Message ID 0x0062 (98).
/// This message is used to deliver the current matrix frame status to the host(s).
/// It can be requested, but is also generated automatically as PSU alarm states transition.
/// </summary>
public class ReplyFrameStatus
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// PSU and frame status flags. Only functional on the HX Delta.
    /// </summary>
    public FramePsuStatus PsuStatus { get; set; }

    /// <summary>
    /// Signed temperature measured at the CPU card in degrees Celsius.
    /// </summary>
    public short CpuCardTemperature { get; set; }

    /// <summary>
    /// Gets whether external PSU 1 has failed.
    /// </summary>
    public bool IsExtPsu1Failed => PsuStatus.HasFlag(FramePsuStatus.ExtPsu1Fail);

    /// <summary>
    /// Gets whether external PSU 2 has failed.
    /// </summary>
    public bool IsExtPsu2Failed => PsuStatus.HasFlag(FramePsuStatus.ExtPsu2Fail);

    /// <summary>
    /// Gets whether internal PSU 1 has failed.
    /// </summary>
    public bool IsIntPsu1Failed => PsuStatus.HasFlag(FramePsuStatus.IntPsu1Fail);

    /// <summary>
    /// Gets whether internal PSU 2 has failed.
    /// </summary>
    public bool IsIntPsu2Failed => PsuStatus.HasFlag(FramePsuStatus.IntPsu2Fail);

    /// <summary>
    /// Gets whether fan 1 has failed.
    /// </summary>
    public bool IsFan1Failed => PsuStatus.HasFlag(FramePsuStatus.Fan1Fail);

    /// <summary>
    /// Gets whether fan 2 has failed.
    /// </summary>
    public bool IsFan2Failed => PsuStatus.HasFlag(FramePsuStatus.Fan2Fail);

    /// <summary>
    /// Gets whether there is a configuration failure.
    /// </summary>
    public bool IsConfigFailed => PsuStatus.HasFlag(FramePsuStatus.ConfigFail);

    /// <summary>
    /// Gets whether external alarm is active.
    /// </summary>
    public bool IsExtAlarmActive => PsuStatus.HasFlag(FramePsuStatus.ExtAlarm);

    /// <summary>
    /// Gets whether there is an over-temperature condition.
    /// </summary>
    public bool IsOvertemp => PsuStatus.HasFlag(FramePsuStatus.Overtemp);

    /// <summary>
    /// Gets whether any alarm condition is active.
    /// </summary>
    public bool HasAnyAlarm => PsuStatus != FramePsuStatus.None;

    /// <summary>
    /// Decodes a Reply Frame Status message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyFrameStatus Decode(byte[] payload)
    {
        var reply = new ReplyFrameStatus();

        if (payload == null || payload.Length < 9)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // PSU Status: 2 bytes (big-endian)
        if (offset + 2 <= payload.Length)
        {
            ushort psuStatusValue = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            reply.PsuStatus = (FramePsuStatus)psuStatusValue;
            offset += 2;
        }

        // CPU Card Temperature: 2 bytes (big-endian, signed)
        if (offset + 2 <= payload.Length)
        {
            reply.CpuCardTemperature = (short)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        return reply;
    }
}
