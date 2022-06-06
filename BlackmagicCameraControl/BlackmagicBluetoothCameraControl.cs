using System.Collections.Concurrent;
using System.Runtime.InteropServices.WindowsRuntime;
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
    }

    public void Dispose()
    {
        _service?.Dispose();
        _device?.Dispose();
    }

    public async ValueTask<bool> SendAsync(CameraControlProtocolMessage message)
    {
        var result = await _outgoingCharacteristic.WriteValueAsync(message.Message.AsBuffer());
        if (result != GattCommunicationStatus.Success)
        {
            Console.WriteLine($"Send Error: {result}");
        }
        return result == GattCommunicationStatus.Success;
    }

    public static async ValueTask<BlackmagicBluetoothCameraControl> CreateAsync(ulong bluetoothAddress, CancellationToken token)
    {
        var device = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);
        token.ThrowIfCancellationRequested();

        var services = await device.GetGattServicesForUuidAsync(BlackmagicCameraServiceGuid);
        if (services.Status != GattCommunicationStatus.Success)
        {
            throw new Exception($"Can't get service. {services.Status}");
        }
        var service = services.Services[0];
        var outgoingCharacteristic = await GetCharacteristicAsync(service, OutgoingCameraControlCharacteristicGuid);
        var incomingCharacteristic = await GetCharacteristicAsync(service, IncomingCameraControlCharacteristicGuid);

        return new BlackmagicBluetoothCameraControl(device, service, outgoingCharacteristic, incomingCharacteristic);
    }


    private static async ValueTask<GattCharacteristic> GetCharacteristicAsync(GattDeviceService service, Guid guid)
    {
        var characters = await service.GetCharacteristicsForUuidAsync(guid);
        if (characters.Status != GattCommunicationStatus.Success)
        {
            throw new Exception($"Can't get characteristics. {characters.Status}");
        }
        return characters.Characteristics[0];
    }
}