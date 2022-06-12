using System.Text;
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
            await using var bluetoothControl = await BlackmagicBluetoothCameraControl.CreateAsync(address, token);
            Console.WriteLine($"CONNECTED to {address}");

            // Read
            bluetoothControl.OnReceived += (message) =>
            {
                var sb = new StringBuilder($"[{message.CommandType.ToCategoryByte()}.{message.CommandType.ToParameterByte()}]{message.CommandType}:");
                var messageString = message switch
                {
                    (_, CommandDataType.Utf8String, _) => $"(String){Encoding.UTF8.GetString(message.CommandData)}",
                    (_, CommandDataType.VoidOrBoolean, 0) => $"(Void)",
                    (_, CommandDataType.VoidOrBoolean, _) => $"(Boolean){Enumerable.Range(0, message.CommandData.Length).Select(x => (message.CommandData[x] != 0).ToString()).JoinString(" ")}",
                    (_, CommandDataType.SInt8, _) => $"(SInt8){Enumerable.Range(0, message.CommandData.Length).Select(x => message.CommandData[x].ToString()).JoinString(" ")}",
                    (_, CommandDataType.SInt16, _) => $"(SInt16){Enumerable.Range(0, message.CommandData.Length / 2).Select(x => BitConverter.ToInt16(message.CommandData, x * 2).ToString()).JoinString(" ")}",
                    (_, CommandDataType.SInt32, _) => $"(SInt32){Enumerable.Range(0, message.CommandData.Length / 4).Select(x => BitConverter.ToInt16(message.CommandData, x * 4).ToString()).JoinString(" ")}",
                    (_, CommandDataType.SInt64, _) => $"(SInt64){Enumerable.Range(0, message.CommandData.Length / 8).Select(x => BitConverter.ToInt16(message.CommandData, x * 8).ToString()).JoinString(" ")}",
                    (_, CommandDataType.SFixedPoint16, _) => $"(SFixedPoint16){Enumerable.Range(0, message.CommandData.Length / 2).Select(x => message.CommandData.FromBlackmagicSignedFixedPoint16(x * 2).ToString()).JoinString(" ")}",
                    _ => message.ToString(),
                };
                sb.Append(messageString);

                Console.WriteLine(sb.ToString());
            };

            // Write
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
                        await bluetoothControl.SendAsync(MessageGenerator.ManualLensFocusRelative(
                            DestinationDeviceType.One,
                            diff * 0.02f));
                    }

                    if (gamepad.LStick_down)
                    {
                        await bluetoothControl.SendAsync(MessageGenerator.AutoFocus(
                            DestinationDeviceType.One));
                    }

                    if (gamepad.Back_down)
                    {
                        await bluetoothControl.SendAsync(MessageGenerator.StartRecording(
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
            Console.WriteLine("STOPPING...");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Catch Exception: {e}");
        }
    }
}
