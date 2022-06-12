namespace BlackmagicCameraControlProtocol;

public enum CommandType : ushort
{
    // Category 0 Lens
    Focus = 0 << 8 | 0,
    InstantaneousAutoFocus = 0 << 8 | 1,
    SetAbsoluteZoomInMillimeter = 0 << 8 | 7,
    SetAbsoluteZoomNormalized = 0 << 8 | 8,

    // Category 10 Media
    TransportMode = 10 << 8 | 1,

    // Category 12 Metadata
    LensFocalLength = 12 << 8 | 11,
    LensDistance = 12 << 8 | 12,
}