using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothLowEnergySandbox
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, ev) =>
            {
                tokenSource.Cancel();
                ev.Cancel = true;
            };
            var token = tokenSource.Token;

            try
            {
                var blackmagicCameraService = new Guid("291d567a-6d75-11e6-8b77-86f30ca893d3");
                var outgoingCameraControlCharacter = new Guid("5DD3465F-1AEE-4299-8493-D2ECA2F8E1BB");
                
                var address = await new BleAdvertisementFinder(blackmagicCameraService).GetBluetoothAddressAsync(token);
                Console.WriteLine(address.ToString()); // 229589057331041
                using (var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address))
                {
                    var services = await device.GetGattServicesForUuidAsync(blackmagicCameraService);
                    if (services.Status != GattCommunicationStatus.Success) throw new Exception($"Can't get service. {services.Status}");
                    var service = services.Services[0];

                    var characters = await service.GetCharacteristicsForUuidAsync(outgoingCameraControlCharacter);
                    if (characters.Status != GattCommunicationStatus.Success) throw new Exception($"Can't get characteristics. {characters.Status}");
                    var character = characters.Characteristics[0];

                    if (!character.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) throw new Exception("Can't write.");

                    var gamepad = XInput.Wrapper.X.Gamepad_1;
                    while (true)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1.0 / 30.0), token);
                        if (token.IsCancellationRequested) break;

                        gamepad.Update();
                        var diff = gamepad.LStick_N.X;
                        await character.SendFocusOrder(diff * 0.02f);
                        Console.WriteLine(diff);

                        // Console.WriteLine("waiting...");
                        // var keyInfo = Console.ReadKey();
                        // if (keyInfo.Key == ConsoleKey.J)
                        // {
                        //     await character.SendFocusOrder(-0.01f);
                        // }
                        // else if (keyInfo.Key == ConsoleKey.K)
                        // {
                        //     await character.SendFocusOrder(+0.01f);
                        // }
                    }
                }
                
                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Catch Exception: {e}");
            }
        }

        private static async Task SendFocusOrder(this GattCharacteristic character, double diff)
        {
            var value = diff.GetBlackmagicSignedFixedPoint16();
            var focusOrder = new byte[]
            {
                0x01, // destination
                0x06, // length
                0x00, 0x00, // reserved
                0x00, 0x00, // category & parameter
                0x80, // type
                0x01, // operation 相対制御
                // 0x33, 0x01,
                value[0], value[1],
                0x00, 0x00,
            };
            
            var autoFocusOrder = new byte[]
            {
                0xFF, // destination
                0x04, // length
                0x00, 0x00, // reserved
                0x00, 0x01, // category & parameter
                0x00, // type
                0x00, // operation
            };
            
            var recordingOrder = new byte[]
            {
                0xFF, // destination
                0x08, // length
                0x00, 0x00, // reserved
                0x0a, 0x01, // category & parameter
                0x01, // type
                0x00, // operation
                0x02, 0x00, 0x00, 0x01,
            };
            
            Console.WriteLine($"Send Focus: {diff} : {focusOrder[8]}, {focusOrder[9]}");

            var result = await character.WriteValueAsync(focusOrder.AsBuffer());
            if (result != GattCommunicationStatus.Success)
            {
                Console.WriteLine($"Send Error: {result}");
            }
        }

        private static byte[] GetBlackmagicSignedFixedPoint16(this double val)
        {
            var x = (ushort) Math.Floor(val * Math.Pow(2, 11));
            return BitConverter.GetBytes(x);
        }
    }
}