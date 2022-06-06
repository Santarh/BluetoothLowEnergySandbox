using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BlackmagicCameraControlProtocol;

namespace BlackmagicCameraControl
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
                Console.WriteLine(address.ToString());
                using (var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address))
                {
                    var services = await device.GetGattServicesForUuidAsync(blackmagicCameraService);
                    if (services.Status != GattCommunicationStatus.Success)
                    {
                        throw new Exception($"Can't get service. {services.Status}");
                    }
                    var service = services.Services[0];

                    var characters = await service.GetCharacteristicsForUuidAsync(outgoingCameraControlCharacter);
                    if (characters.Status != GattCommunicationStatus.Success)
                    {
                        throw new Exception($"Can't get characteristics. {characters.Status}");
                    }
                    var character = characters.Characteristics[0];

                    if (!character.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write))
                    {
                        throw new Exception("Can't write.");
                    }

                    Console.WriteLine($"CONNECTED to {address}");

                    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1.0 / 30.0));
                    var gamepad = XInput.Wrapper.X.Gamepad_1;
                    while (true)
                    {
                        try
                        {
                            await timer.WaitForNextTickAsync(token);
                            if (token.IsCancellationRequested) break;

                            gamepad.Update();

                            var messageQueue = new Queue<CameraControlProtocolMessage>();
                            // focus
                            var diff = gamepad.LStick_N.X;
                            if (Math.Abs(diff) > 0.01f)
                            {
                                messageQueue.Enqueue(CameraControlProtocolMessage.ManualLensFocusRelative(
                                    DestinationDeviceType.One,
                                    diff * 0.02f));
                            }

                            if (gamepad.LStick_down)
                            {
                                messageQueue.Enqueue(CameraControlProtocolMessage.AutoFocus(DestinationDeviceType.One));
                            }

                            if (gamepad.Back_down)
                            {
                                messageQueue.Enqueue(CameraControlProtocolMessage.StartRecording(DestinationDeviceType.One));
                            }

                            while (messageQueue.Count > 0)
                            {
                                var message = messageQueue.Dequeue();
                                Console.WriteLine($"Sending... {message}");
                                var result = await character.WriteValueAsync(message.Message.AsBuffer());
                                if (result != GattCommunicationStatus.Success)
                                {
                                    Console.WriteLine($"Send Error: {result}");
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Catch Exception: {e}");
                        }
                    }
                }

                Console.WriteLine("Done");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("CANCELED!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Catch Exception: {e}");
            }
        }
    }
}
