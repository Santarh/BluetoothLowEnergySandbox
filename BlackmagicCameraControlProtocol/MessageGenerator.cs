namespace BlackmagicCameraControlProtocol;

public static class MessageGenerator
{
    private static CameraControlProtocolMessage GenerateMessage(DestinationDeviceType dstType, CommandType commandType, byte[] commandData)
    {
        var commandSpec = CommandSpecification.Get(commandType);
        return new CameraControlProtocolMessage(
            dstType,
            commandSpec.CommandLength,
            commandSpec.Id,
            commandSpec.Type,
            commandSpec.DataType,
            commandSpec.OperationType,
            commandData
        );
    }

    public static CameraControlProtocolMessage ManualLensFocusRelative(DestinationDeviceType dst, double offsetValue)
    {
        return GenerateMessage(
            dst,
            CommandType.Focus,
            Math.Max(-1.0, Math.Min(+1.0, offsetValue)).AsBlackmagicSignedFixedPoint16()
        );
    }

    public static CameraControlProtocolMessage AutoFocus(DestinationDeviceType dst)
    {
        return GenerateMessage(
            dst,
            CommandType.InstantaneousAutoFocus,
            Array.Empty<byte>()
        );
    }

    public static CameraControlProtocolMessage StartRecording(DestinationDeviceType dst)
    {
        return GenerateMessage(
            dst,
            CommandType.TransportMode,
            new byte[] { 2, 0, 0, 0, 0 }
        );
    }
}
