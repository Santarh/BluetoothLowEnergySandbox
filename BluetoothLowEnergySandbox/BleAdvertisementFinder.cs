using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace BluetoothLowEnergySandbox
{
    public sealed class BleAdvertisementFinder
    {
        private readonly Guid _targetServiceGuid;

        public BleAdvertisementFinder(Guid targetServiceGuid)
        {
            _targetServiceGuid = targetServiceGuid;
        }
        
        public async Task<ulong> GetBluetoothAddressAsync(CancellationToken token)
        {
            var watcher = new BluetoothLEAdvertisementWatcher();
            watcher.ScanningMode = BluetoothLEScanningMode.Passive;

            ulong? result = null;
            watcher.Received += (sender, args) => OnReceived(ref result, sender, args);
            watcher.Start();
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), token);
                if (token.IsCancellationRequested) break;
                if (result.HasValue) break;
            }
            watcher.Stop();

            return result ?? default;
        }

        private void OnReceived(ref ulong? result, BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            if (result.HasValue) return;
            
            Output(args);
            foreach (var uuid in args.Advertisement.ServiceUuids)
            {
                if (uuid.Equals(_targetServiceGuid))
                {
                    result = args.BluetoothAddress;
                    return;
                }
            }
        }

        private static void Output(BluetoothLEAdvertisementReceivedEventArgs args)
        {
            Console.WriteLine($"ADDR: {args.BluetoothAddress}");
            Console.WriteLine($"Services:");
            foreach (var uuid in args.Advertisement.ServiceUuids)
            {
                Console.WriteLine($"    {uuid.ToString()}");
            }
            Console.WriteLine();
        }
    }
}