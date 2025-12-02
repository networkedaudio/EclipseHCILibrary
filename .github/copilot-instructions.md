<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# HCILibrary Project

This is a C# .NET 8 library for HCI (Human-Computer Interface) protocol communication over TCP.

## Project Structure

- `HCIConnection.cs` - Manages TCP connection with automatic port failover (52020-52001)
- `HCIResponse.cs` - Decodes incoming HCI messages
- `HCIRequestQueue.cs` - Rate-limited queue for outgoing requests
- `Models/` - Data models (HCIReply, HCIRequest, HCIFlags)
- `Enums/` - Enumerations (HCIVersion, HCIMessageID)

## Message Format

- Start marker: `0x5A 0x0F`
- End marker: `0x2E 0x8D`
- Length field: 2 bytes (big-endian) after start marker
- Message ID: 2 bytes (big-endian)
- Flags: 1 byte (E, M, U, G, S, N in bits 0-5)
- HCIv2 marker: `0xAB 0xBA 0xCE 0xDE` (optional)
- Additional protocol indicator: 1 byte (HCIv2 only)
- Payload: variable length
- End marker: 2 bytes

## Adding New Message Types

1. Add the message ID to `Enums/HCIMessageID.cs`
2. Create a decoder in `HCIResponse.cs` under the appropriate version switch
3. For requests, inherit from `HCIRequest` and override `GeneratePayload()`
