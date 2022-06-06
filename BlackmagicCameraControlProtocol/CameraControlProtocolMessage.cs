using System.Text;

namespace BlackmagicCameraControlProtocol
{
    public sealed class CameraControlProtocolMessage
    {
        public byte[] Message { get; }

        internal CameraControlProtocolMessage(
            DestinationDeviceType destinationDeviceType,
            Command command,
            byte[] data
        )
        {
            // padding with 32bit boundary
            var length = command.Length;
            var messageByteCount = 4 + (length / 4 + (length % 4 == 0 ? 0 : 1)) * 4;
            var message = new byte[messageByteCount];
            
            // Header
            message[0] = (byte) destinationDeviceType;
            message[1] = length;
            message[2] = command.Id; // command id
            message[3] = 0x00; // reserved
            
            // Command
            message[4] = command.Category; // category
            message[5] = command.Parameter; // parameter
            message[6] = command.DataType; // data type
            message[7] = command.OperationType; // operation
            
            // Command Data 
            var requiredDataLength = command.Length - 4;
            if (data.Length != requiredDataLength) throw new ArgumentException($"Invalid {nameof(data)} length.");
            for (var idx = 0; idx < requiredDataLength; ++idx)
            {
                message[8 + idx] = data[idx];
            }

            Message = message;
        }

        public CameraControlProtocolMessage(byte[] data)
        {
            Message = data;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("0x ");
            foreach (var b in Message)
            {
                sb.AppendFormat("{0:X2} ", b);
            }
            sb.Append($"({Message.Length} bytes)");
            return sb.ToString();
        }

        public static CameraControlProtocolMessage ManualLensFocusRelative(DestinationDeviceType dst, double offsetValue)
        {
            return new CameraControlProtocolMessage(
                dst,
                Command.ManualLensFocusRelative,
                Math.Max(-1.0, Math.Min(+1.0, offsetValue)).AsBlackmagicSignedFixedPoint16()
            );
        }

        public static CameraControlProtocolMessage AutoFocus(DestinationDeviceType dst)
        {
            return new CameraControlProtocolMessage(
                dst,
                Command.AutoLensFocus,
                new byte[0]
            );
        }

        public static CameraControlProtocolMessage StartRecording(DestinationDeviceType dst)
        {
            return new CameraControlProtocolMessage(
                dst,
                Command.ChangeTransportMode,
                new byte[] {2, 0, 0, 0, 0}
            );
        }
    }
}