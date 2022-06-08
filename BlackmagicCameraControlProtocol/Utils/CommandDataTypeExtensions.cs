namespace BlackmagicCameraControlProtocol;

internal static class CommandDataTypeExtensions
{
    public static byte GetByteSize(this CommandDataType type)
    {
        return type switch
        {
            CommandDataType.Void => 0,
            CommandDataType.SInt8 => 1,
            CommandDataType.SInt16 => 2,
            CommandDataType.SInt32 => 4,
            CommandDataType.SInt64 => 8,
            CommandDataType.Utf8String => 1,
            CommandDataType.SFixedPoint16 => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}