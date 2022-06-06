namespace BlackmagicCameraControlProtocol
{
    internal readonly struct Command
    {
        /// <summary>
        /// コマンド ID. いまのところ 0 番 ID のコマンドしか存在しないっぽい.
        /// </summary>
        public byte Id { get; }
        
        /// <summary>
        /// コマンドチャンクの byte 長.
        /// 先頭 4 byte と padding の zero-fill は含めない.
        /// </summary>
        public byte Length { get; }
        
        /// <summary>
        /// コマンドのカテゴリ
        /// </summary>
        public byte Category { get; }
        
        /// <summary>
        /// コマンドのパラメタ
        /// </summary>
        public byte Parameter { get; }
        
        /// <summary>
        /// コマンドのデータの型
        /// </summary>
        public byte DataType { get; }
        
        /// <summary>
        /// コマンドの操作タイプ
        /// </summary>
        public byte OperationType { get; }

        public Command(byte id, byte length, byte category, byte parameter, CommandDataType commandDataType, CommandOperationType commandOperationType)
        {
            Id = id;
            Length = length;
            Category = category;
            Parameter = parameter;
            DataType = (byte) commandDataType;
            OperationType = (byte) commandOperationType;
        }
        
        public static Command ManualLensFocusRelative => new Command(0, 6, 0, 0, CommandDataType.SFixedPoint16, CommandOperationType.OffsetValue);
        public static Command AutoLensFocus => new Command(0, 4, 0, 1, CommandDataType.Void, CommandOperationType.AssignValue);
        public static Command ChangeTransportMode => new Command(0, 9, 10, 1, CommandDataType.SInt8, CommandOperationType.AssignValue);
    }
}