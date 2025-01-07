using MessagePack;

namespace YAMDCC.IPC;

internal static class Constants
{
    public static readonly MessagePackSerializerOptions SerializerOptions =
        MessagePackSerializerOptions.Standard.WithSecurity(MessagePackSecurity.TrustedData);
}
