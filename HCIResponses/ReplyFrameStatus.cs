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
    /// Temperature measured at the CPU card in degrees Celsius (0.5° resolution).
    /// </summary>
    public double CpuCardTemperature { get; set; }

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
    /// <param name="payload">The message payload (after flags and protocol schema).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyFrameStatus Decode(byte[] payload)
    {
        var reply = new ReplyFrameStatus();

        if (payload == null || payload.Length < 4)
            return reply;

        System.Diagnostics.Debug.WriteLine($"ReplyFrameStatus payload ({payload.Length} bytes): {BitConverter.ToString(payload)}");

        int offset = 0;

        // PSU Status: 2 bytes (big-endian)
        ushort psuStatusValue = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        reply.PsuStatus = (FramePsuStatus)psuStatusValue;
        offset += 2;

        // CPU Card Temperature: 2 bytes (big-endian, fixed-point with 0.5° resolution)
        // Upper byte = signed integer degrees, lower byte bit 7 = 0.5° fractional part
        ushort rawTemp = (ushort)((payload[offset] << 8) | payload[offset + 1]);

        double fractional = (rawTemp & 0x0080) != 0 ? 0.5 : 0.0;

        int integerPart = rawTemp >> 8;

        // Sign-extend if bit 7 of the integer part is set (negative temperature)
        if ((integerPart & 0x80) != 0)
            integerPart |= unchecked((int)0xFFFFFF00);

        reply.CpuCardTemperature = integerPart + fractional;

        return reply;
    }
}
