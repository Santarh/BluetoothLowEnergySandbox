using BlackmagicCameraControlProtocol;

namespace BlackmagicCameraControl;

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
            var address = await new BleAdvertisementFinder(BlackmagicBluetoothCameraControl.BlackmagicCameraServiceGuid).GetBluetoothAddressAsync(token);
            using var bluetoothControl = await BlackmagicBluetoothCameraControl.CreateAsync(address, token);
            Console.WriteLine($"CONNECTED to {address}");

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1.0 / 10.0));
            var gamepad = XInput.Wrapper.X.Gamepad_1;
            while (true)
            {
                try
                {
                    await timer.WaitForNextTickAsync(token);
                    if (token.IsCancellationRequested) break;

                    gamepad.Update();

                    // focus
                    var diff = gamepad.LStick_N.X;
                    if (Math.Abs(diff) > 0.01f)
                    {
                        await bluetoothControl.SendAsync(CameraControlProtocolMessage.ManualLensFocusRelative(
                            DestinationDeviceType.One,
                            diff * 0.02f));
                    }

                    if (gamepad.LStick_down)
                    {
                        await bluetoothControl.SendAsync(CameraControlProtocolMessage.AutoFocus(
                            DestinationDeviceType.One));
                    }

                    if (gamepad.Back_down)
                    {
                        await bluetoothControl.SendAsync(CameraControlProtocolMessage.StartRecording(
                            DestinationDeviceType.One));
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
