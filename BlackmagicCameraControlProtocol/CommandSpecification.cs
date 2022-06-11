namespace BlackmagicCameraControlProtocol;

public sealed class CommandSpecification
{
    /// <summary>
    /// コマンド ID. いまのところ 0 番 ID のコマンドしか存在しないっぽい.
    /// </summary>
    public byte Id { get; }

    /// <summary>
    /// コマンドのカテゴリとパラメタ
    /// </summary>
    public CommandType Type { get; }

    /// <summary>
    /// コマンドのデータの型
    /// </summary>
    public CommandDataType DataType { get; }

    /// <summary>
    /// コマンドのデータの配列数
    /// </summary>
    public byte DataCount { get; }

    /// <summary>
    /// コマンドの操作タイプ
    /// </summary>
    public CommandOperationType OperationType { get; }

    /// <summary>
    /// コマンドチャンクの長さ
    /// </summary>
    public byte CommandLength => (byte)(4 + DataType.GetByteSize() * DataCount);

    public CommandSpecification(byte id, CommandType type, CommandDataType dataType, byte dataCount, CommandOperationType operationType)
    {
        Id = id;
        Type = type;
        DataType = dataType;
        DataCount = dataCount;
        OperationType = operationType;
    }

    public static CommandSpecification Get(CommandType type)
    {
        if (AllCommands.ContainsKey(type))
        {
            return AllCommands[type];
        }

        throw new ArgumentException();
    }

    public static IReadOnlyDictionary<CommandType, CommandSpecification> AllCommands => _allCommands;

    private static readonly Dictionary<CommandType, CommandSpecification> _allCommands =
        new Dictionary<CommandType, CommandSpecification>()
        {
            { CommandType.Focus , new CommandSpecification(0, CommandType.Focus, CommandDataType.SFixedPoint16, 1, CommandOperationType.OffsetValue)},
            { CommandType.InstantaneousAutoFocus, new CommandSpecification(0, CommandType.InstantaneousAutoFocus, CommandDataType.Void, 0, CommandOperationType.AssignValue)},
            { CommandType.SetAbsoluteZoomInMillimeter, new CommandSpecification(0, CommandType.SetAbsoluteZoomInMillimeter, CommandDataType.SInt16, 1, CommandOperationType.AssignValue)},
            { CommandType.TransportMode, new CommandSpecification(0, CommandType.TransportMode, CommandDataType.SInt8, 5, CommandOperationType.AssignValue)},
        };
}