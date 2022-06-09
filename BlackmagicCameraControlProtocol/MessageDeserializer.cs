namespace BlackmagicCameraControlProtocol;

public static class MessageDeserializer
{
    public static bool TryDeserialize(byte[] data, out CameraControlProtocolMessage message)
    {
        try
        {
            message = Deserialize(data);
            return true;
        }
        catch (Exception)
        {
            message = default;
            return false;
        }
    }

    public static CameraControlProtocolMessage Deserialize(byte[] data)
    {
        if (data == null || data.Length < MessageSerializer.MessageMinimumByteCount)
        {
            throw new ArgumentException();
        }

        // Header
        var destinationDevice = (DestinationDeviceType)data[0];
        var commandByteCount = data[1];
        var commandId = data[2];

        // Command
        var commandType = (CommandType)(data[4] << 8 | data[5]);
        var dataType = (CommandDataType)data[6];
        var operationType = (CommandOperationType)data[7];

        // CommandData
        var commandDataByteCount = commandByteCount - MessageSerializer.CommandHeaderByteCount;
        if (data.Length < MessageSerializer.MessageMinimumByteCount + commandDataByteCount)
        {
            throw new ArgumentException();
        }
        var commandData = new byte[commandDataByteCount];
        for (var idx = 0; idx < commandDataByteCount; ++idx)
        {
            commandData[idx] = data[MessageSerializer.MessageMinimumByteCount + idx];
        }

        return new CameraControlProtocolMessage(
            destinationDevice,
            commandByteCount,
            commandId,
            commandType,
            dataType,
            operationType,
            commandData
        );
    }
}