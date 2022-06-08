namespace BlackmagicCameraControlProtocol;

internal static class CommandTypeExtensions
{
    public static byte ToCategoryByte(this CommandType type)
    {
        return (byte)((ushort)type >> 8);
    }

    public static byte ToParameterByte(this CommandType type)
    {
        return (byte)((ushort)type & 0x00FF);
    }
}