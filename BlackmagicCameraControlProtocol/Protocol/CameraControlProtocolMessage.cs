namespace BlackmagicCameraControlProtocol;

public readonly struct CameraControlProtocolMessage
{
    /// <summary>
    /// 目的とするデバイス ID
    /// ブロードキャストの場合は 255
    /// </summary>
    public DestinationDeviceType DestinationDevice { get; }

    /// <summary>
    /// コマンドチャンクの長さ
    /// 末尾のパディングは含めない
    /// </summary>
    public byte CommandLength { get; }

    /// <summary>
    /// コマンドセットの ID
    /// 現状ドキュメントには ID 0 のコマンドしか書いていない
    /// </summary>
    public byte CommandId { get; }

    /// <summary>
    /// ここからコマンドチャンク
    /// コマンドのカテゴリとパラメタ
    /// </summary>
    public CommandType CommandType { get; }

    /// <summary>
    /// コマンドのデータの型
    /// </summary>
    public CommandDataType DataType { get; }

    /// <summary>
    /// コマンドの操作タイプ
    /// </summary>
    public CommandOperationType OperationType { get; }

    /// <summary>
    /// コマンドに対応するデータ
    /// </summary>
    public byte[] CommandData { get; }

    public CameraControlProtocolMessage(DestinationDeviceType destinationDevice, byte commandLength, byte commandId, CommandType commandType, CommandDataType dataType, CommandOperationType operationType, byte[] commandData)
    {
        DestinationDevice = destinationDevice;
        CommandLength = commandLength;
        CommandId = commandId;
        CommandType = commandType;
        DataType = dataType;
        OperationType = operationType;
        CommandData = commandData;
    }
}