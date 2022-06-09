namespace BlackmagicCameraControlProtocol;

public enum CommandType : ushort
{
    Focus = 0 << 8 | 0,
    InstantaneousAutoFocus = 0 << 8 | 1,
    SetAbsoluteZoomInMillimeter = 0 << 8 | 7,
    SetAbsoluteZoomNormalized = 0 << 8 | 8,

    TransportMode = 10 << 8 | 1,
}