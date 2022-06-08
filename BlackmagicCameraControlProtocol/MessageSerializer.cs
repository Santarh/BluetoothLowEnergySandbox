namespace BlackmagicCameraControlProtocol;

public static class MessageSerializer
{
    public static byte[] Serialize(CameraControlProtocolMessage message)
    {
        var commandByteCount = message.CommandLength;
        var messageByteCount = 4 + commandByteCount;
        // calculate data length by padding with 4byte boundary
        var dataByteCount = (messageByteCount / 4 + (messageByteCount % 4 == 0 ? 0 : 1)) * 4;

        if (dataByteCount > 64) // spec
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
        var commandDataByteCount = commandByteCount - 4;
        for (var idx = 0; idx < commandDataByteCount; ++idx)
        {
            data[8 + idx] = message.CommandData[idx];
        }

        return data;
    }
}