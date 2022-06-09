namespace BlackmagicCameraControlProtocol;

public static class MessageSerializer
{
    public const int MessageHeaderByteCount = 4;
    public const int CommandHeaderByteCount = 4;
    public const int Padding = 4;
    public const int MessageMaximumByteCount = 64;
    public const int MessageMinimumByteCount = MessageHeaderByteCount + CommandHeaderByteCount;

    public static bool TrySerialize(CameraControlProtocolMessage message, out byte[] data)
    {
        try
        {
            data = Serialize(message);
            return true;
        }
        catch (Exception)
        {
            data = default;
            return false;
        }
    }

    public static byte[] Serialize(CameraControlProtocolMessage message)
    {
        var commandByteCount = message.CommandByteCount;
        var messageByteCount = MessageHeaderByteCount + commandByteCount;
        // calculate data length by padding with 4byte boundary
        var dataByteCount = (messageByteCount / Padding + (messageByteCount % Padding == 0 ? 0 : 1)) * Padding;

        if (dataByteCount > MessageMaximumByteCount)
        {
            throw new ArgumentException("Too long message.");
        }

        var data = new byte[dataByteCount];

        // Header
        data[0] = (byte) message.DestinationDevice;
        data[1] = (byte) commandByteCount;
        data[2] = message.CommandId;
        data[3] = 0x00; // reserved

        // Command
        data[4] = message.CommandType.ToCategoryByte();
        data[5] = message.CommandType.ToParameterByte();
        data[6] = (byte)message.DataType;
        data[7] = (byte)message.OperationType;

        // Command Data
        var commandDataByteCount = commandByteCount - CommandHeaderByteCount;
        if (commandDataByteCount > 0)
        {
            if (message.CommandData.Length != commandDataByteCount)
            {
                throw new ArgumentException();
            }

            for (var idx = 0; idx < commandDataByteCount; ++idx)
            {
                data[MessageMinimumByteCount + idx] = message.CommandData[idx];
            }
        }

        return data;
    }
}