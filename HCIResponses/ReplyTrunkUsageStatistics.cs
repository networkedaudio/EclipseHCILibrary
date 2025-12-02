using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Trunk Usage Statistics (HCIv2) - Message ID 0x0171 (369).
/// Contains a report on the current trunk statistics held by the matrix.
/// The information is equivalent to that displayed every 10 minutes in the EHX event log.
/// </summary>
public class ReplyTrunkUsageStatistics
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The local matrix ID used to determine which field type to use for Fibre Shared Resources entries.
    /// This should be set before decoding if you want to distinguish between Type 1 and Type 2 entries.
    /// </summary>
    public byte LocalMatrixId { get; set; }

    /// <summary>
    /// List of trunk usage statistics entries (Type 1).
    /// Contains standard trunk statistics for fibre, Tx, and reserved trunks.
    /// </summary>
    public List<TrunkUsageStatisticsEntry> Entries { get; set; } = new();

    /// <summary>
    /// List of fibre shared resources entries (Type 2).
    /// Contains fibre resource statistics when Matrix ID matches the local matrix.
    /// </summary>
    public List<TrunkUsageStatisticsFibreEntry> FibreEntries { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyTrunkUsageStatistics.
    /// Note: All entries are decoded as Type 1 entries. To properly decode Type 2 entries,
    /// use the overload that accepts the local matrix ID.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyTrunkUsageStatistics Decode(byte[] payload)
    {
        return Decode(payload, 0xFF); // Use invalid matrix ID so all entries decode as Type 1
    }

    /// <summary>
    /// Decodes the payload into a ReplyTrunkUsageStatistics.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <param name="localMatrixId">The local matrix ID to determine Type 2 entries.</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyTrunkUsageStatistics Decode(byte[] payload, byte localMatrixId)
    {
        var reply = new ReplyTrunkUsageStatistics
        {
            LocalMatrixId = localMatrixId
        };

        if (payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 2 bytes (big-endian)
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse entries (12 bytes per record)
        for (int i = 0; i < count && offset + 12 <= payload.Length; i++)
        {
            byte matrixId = payload[offset++];
            TrunkStatisticsType statisticsType = (TrunkStatisticsType)payload[offset++];

            // Check if this is a Type 2 entry (Fibre Shared Resources for local matrix)
            bool isType2 = statisticsType == TrunkStatisticsType.FibreSharedResources && matrixId == localMatrixId;

            if (isType2)
            {
                var fibreEntry = new TrunkUsageStatisticsFibreEntry
                {
                    MatrixId = matrixId,
                    StatisticsType = statisticsType,
                    AllocatedRxResources = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                    PeakRxResources = (ushort)((payload[offset + 2] << 8) | payload[offset + 3]),
                    TdmPortsAllocated = (ushort)((payload[offset + 4] << 8) | payload[offset + 5]),
                    PeakTdmPortsAllocated = (ushort)((payload[offset + 6] << 8) | payload[offset + 7]),
                    TotalTdmPorts = (ushort)((payload[offset + 8] << 8) | payload[offset + 9])
                };
                offset += 10;
                reply.FibreEntries.Add(fibreEntry);
            }
            else
            {
                var entry = new TrunkUsageStatisticsEntry
                {
                    MatrixId = matrixId,
                    StatisticsType = statisticsType,
                    InUseCount = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                    TenMinPeak = (ushort)((payload[offset + 2] << 8) | payload[offset + 3]),
                    TenMinMean = (ushort)((payload[offset + 4] << 8) | payload[offset + 5]),
                    PeakSinceStart = (ushort)((payload[offset + 6] << 8) | payload[offset + 7]),
                    Total = (ushort)((payload[offset + 8] << 8) | payload[offset + 9])
                };
                offset += 10;
                reply.Entries.Add(entry);
            }
        }

        return reply;
    }
}
