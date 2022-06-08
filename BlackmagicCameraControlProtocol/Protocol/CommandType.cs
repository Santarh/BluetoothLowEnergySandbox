namespace BlackmagicCameraControlProtocol;

public enum CommandType : ushort
{
    Focus = 0 << 8 | 0,
    InstantaneousAutoFocus = 0 << 8 | 1,

    TransportMode = 10 << 8 | 1,
}