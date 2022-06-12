namespace BlackmagicCameraControlProtocol;

public enum CommandType : ushort
{
    // Category 0 Lens
    Focus = 0 << 8 | 0,
    InstantaneousAutoFocus = 0 << 8 | 1,
    ApertureInFStop = 0 << 8 | 2,
    ApertureInNormalized = 0 << 8 | 3,
    ApertureInOrdinal = 0 << 8 | 4,
    InstantaneousAutoAperture = 0 << 8 | 5,
    OpticalImageStabilisation = 0 << 8 | 6,
    SetAbsoluteZoomInMillimeter = 0 << 8 | 7,
    SetAbsoluteZoomInNormalized = 0 << 8 | 8,
    SetContinuousZoomInSpeed = 0 << 8 | 9,

    // Category 1 Video
    VideoMode = 1 << 8 | 0,
    ObsoletedGain = 1 << 8 | 1,
    ManualWhiteBalance = 1 << 8 | 2,
    SetAutoWb = 1 << 8 | 3,
    RestoreAutoWb = 1 << 8 | 4,
    ExposureInMicroSeconds = 1 << 8 | 5,
    ExposureInOrdinal = 1 << 8 | 6,
    DynamicRangeMode = 1 << 8 | 7,
    VideoSharpeningLevel = 1 << 8 | 8,
    RecordingFormat = 1 << 8 | 9,
    SetAutoExposureMode = 1 << 8 | 10,
    ShutterAngle = 1 << 8 | 11,
    ShutterSpeed = 1 << 8 | 12,
    Gain = 1 << 8 | 13,
    Iso = 1 << 8 | 14,
    DisplayLut = 1 << 8 | 15,
    NdFilter = 1 << 8 | 16,

    // Category 2 Audio
    HeadphoneLevel = 2 << 8 | 1,
    HeadphoneProgramMix = 2 << 8 | 2,
    SpeakerLevel = 2 << 8 | 3,
    InputType = 2 << 8 | 4,
    InputLevels = 2 << 8 | 5,
    PhantomPower = 2 << 8 | 6,

    // Category 3 Output
    OverlayEnables = 3 << 8 | 0,
    ObsoletedFrameGuidesStyle = 3 << 8 | 1,
    ObsoletedFrameGuidesOpacity = 3 << 8 | 2,
    Overlays = 3 << 8 | 3,

    // Category 4 Display

    // Category 5 Tally

    // Category 6 Reference

    // Category 7 Configuration
    RealTimeClock = 7 << 8 | 0,
    SystemLanguage = 7 << 8 | 1,
    Timezone = 7 << 8 | 2,
    Location = 7 << 8 | 3,

    // Category 8 Color Correction

    // Category 9 Undocumented
    AmbiguousKeepAlive = 9 << 8 | 0,
    AmbiguousRecordingStatus = 9 << 8 | 2,

    // Category 10 Media
    Codec = 10 << 8 | 0,
    TransportMode = 10 << 8 | 1,
    PlaybackControl = 10 << 8 | 2,
    StillCapture = 10 << 8 | 3,

    // Category 11 PTZ Control
    PanTiltVelocity = 11 << 8 | 0,
    MemoryPreset = 11 << 8 | 1,

    // Category 12 Metadata
    Reel = 12 << 8 | 0,
    SceneTags = 12 << 8 | 1,
    Scene = 12 << 8 | 2,
    Take = 12 << 8 | 3,
    GoodTake = 12 << 8 | 4,
    CameraId = 12 << 8 | 5,
    CameraOperator = 12 << 8 | 6,
    Director = 12 << 8 | 7,
    ProjectName = 12 << 8 | 8,
    LensType = 12 << 8 | 9,
    LensIris = 12 << 8 | 10,
    LensFocalLength = 12 << 8 | 11,
    LensDistance = 12 << 8 | 12,
    LensFilter = 12 << 8 | 13,
    SlateMode = 12 << 8 | 14,
    SlateTarget = 12 << 8 | 15,
}