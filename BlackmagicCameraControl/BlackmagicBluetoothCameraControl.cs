using System.Collections.Concurrent;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BlackmagicCameraControlProtocol;

namespace BlackmagicCameraControl;

public sealed class BlackmagicBluetoothCameraControl : IDisposable
{
    public static readonly Guid BlackmagicCameraServiceGuid = new Guid("291d567a-6d75-11e6-8b77-86f30ca893d3");
    public static readonly Guid OutgoingCameraControlCharacteristicGuid = new Guid("5DD3465F-1AEE-4299-8493-D2ECA2F8E1BB");
    public static readonly Guid IncomingCameraControlCharacteristicGuid = new Guid("B864E140-76A0-416A-BF30-5876504537D9");

    private readonly BluetoothLEDevice _device;
    private readonly GattDeviceService _service;
    private readonly GattCharacteristic _outgoingCharacteristic;
    private readonly GattCharacteristic _incomingCharacteristic;

    private BlackmagicBluetoothCameraControl(BluetoothLEDevice device, GattDeviceService service,
        GattCharacteristic outgoingCharacteristic, GattCharacteristic incomingCharacteristic)
    {
        _device = device;
        _service = service;
        _outgoingCharacteristic = outgoingCharacteristic;
        _incomingCharacteristic = incomingCharacteristic;

        _incomingCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
            GattClientCharacteristicConfigurationDescriptorValue.Indicate);
        _incomingCharacteristic.ValueChanged += (characteristic, args) =>
        {
            // var message = new CameraControlProtocolMessage(args.CharacteristicValue.ToArray());
            // Console.WriteLine(message);
        };
    }

    public void Dispose()
    {
        _service?.Dispose();
        _device?.Dispose();
    }

    public async ValueTask<bool> SendAsync(CameraControlProtocolMessage message)
    {
        Console.WriteLine($"Sending... {message}");
        var result = await _outgoingCharacteristic.WriteValueAsync(MessageSerializer.Serialize(message).AsBuffer());
        if (result != GattCommunicationStatus.Success)
        {
            Console.WriteLine($"Send Error: {result}");
        }
        return result == GattCommunicationStatus.Success;
    }

    public static async ValueTask<BlackmagicBluetoothCameraControl> CreateAsync(ulong bluetoothAddress, CancellationToken token)
    {
        var device = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);
        var service = await GetFirstServiceAsync(device, BlackmagicCameraServiceGuid);
        var outgoingCharacteristic = await GetFirstCharacteristicAsync(service, OutgoingCameraControlCharacteristicGuid);
        var incomingCharacteristic = await GetFirstCharacteristicAsync(service, IncomingCameraControlCharacteristicGuid);

        return new BlackmagicBluetoothCameraControl(device, service, outgoingCharacteristic, incomingCharacteristic);
    }

    private static async ValueTask<GattDeviceService> GetFirstServiceAsync(BluetoothLEDevice device, Guid guid)
    {
        var services = await device.GetGattServicesForUuidAsync(guid);
        if (services.Status != GattCommunicationStatus.Success)
        {
            throw new Exception($"Can't get service. {services.Status}");
        }
        return services.Services[0];
    }


    private static async ValueTask<GattCharacteristic> GetFirstCharacteristicAsync(GattDeviceService service, Guid guid)
    {
        var characters = await service.GetCharacteristicsForUuidAsync(guid);
        if (characters.Status != GattCommunicationStatus.Success)
        {
            throw new Exception($"Can't get characteristics. {characters.Status}");
        }
        return characters.Characteristics[0];
    }
}