namespace BlackmagicCameraControlProtocol;

public enum CommandDataType : byte
{
    Void = 0,
    Boolean = 0,
    SInt8 = 1,
    SInt16 = 2,
    SInt32 = 3,
    SInt64 = 4,
    Utf8String = 5,
    SFixedPoint16 = 128,
}