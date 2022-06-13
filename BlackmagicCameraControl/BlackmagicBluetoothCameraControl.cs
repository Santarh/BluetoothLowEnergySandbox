using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BlackmagicCameraControlProtocol;

namespace BlackmagicCameraControl;

public delegate void ControlMessageEventHandler(CameraControlProtocolMessage message);

public sealed class BlackmagicBluetoothCameraControl : IAsyncDisposable
{
    public static readonly Guid BlackmagicCameraServiceGuid = new Guid("291d567a-6d75-11e6-8b77-86f30ca893d3");
    public static readonly Guid OutgoingCameraControlCharacteristicGuid = new Guid("5DD3465F-1AEE-4299-8493-D2ECA2F8E1BB");
    public static readonly Guid IncomingCameraControlCharacteristicGuid = new Guid("B864E140-76A0-416A-BF30-5876504537D9");

    private readonly BluetoothLEDevice _device;
    private readonly GattDeviceService _service;
    private readonly GattCharacteristic _outgoingCharacteristic;
    private readonly GattCharacteristic _incomingCharacteristic;

    public event ControlMessageEventHandler OnReceived;

    public static async ValueTask<BlackmagicBluetoothCameraControl> CreateAsync(ulong bluetoothAddress, CancellationToken token)
    {
        var device = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);
        var service = await GetFirstServiceAsync(device, BlackmagicCameraServiceGuid);
        var outgoingCharacteristic = await GetFirstCharacteristicAsync(service, OutgoingCameraControlCharacteristicGuid);
        var incomingCharacteristic = await GetFirstCharacteristicAsync(service, IncomingCameraControlCharacteristicGuid);

        var self = new BlackmagicBluetoothCameraControl(device, service, outgoingCharacteristic, incomingCharacteristic);

        incomingCharacteristic.ValueChanged += self.OnGattValueChanged;
        // NOTE: たまに Descriptor 書き込みで COMException が出るので少し待つ（効果不明）
        await Task.Delay(TimeSpan.FromSeconds(0.2), token);
        var result = await incomingCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
        if (result != GattCommunicationStatus.Success)
        {
            throw new Exception($"Can't write indicate status. {result}");
        }

        return self;
    }

    private BlackmagicBluetoothCameraControl(BluetoothLEDevice device, GattDeviceService service,
        GattCharacteristic outgoingCharacteristic, GattCharacteristic incomingCharacteristic)
    {
        _device = device;
        _service = service;
        _outgoingCharacteristic = outgoingCharacteristic;
        _incomingCharacteristic = incomingCharacteristic;
    }

    public async ValueTask DisposeAsync()
    {
        _incomingCharacteristic.ValueChanged -= OnGattValueChanged;
        await _incomingCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);

        _service?.Dispose();
        _device?.Dispose();
    }

    public async ValueTask<bool> SendAsync(CameraControlProtocolMessage message)
    {
        Console.WriteLine($"Sending... {message}");
        var result = await _outgoingCharacteristic.WriteValueAsync(
            MessageSerializer.Serialize(message).AsBuffer(),
            GattWriteOption.WriteWithResponse);
        if (result != GattCommunicationStatus.Success)
        {
            Console.WriteLine($"Send Error: {result}");
        }
        return result == GattCommunicationStatus.Success;
    }

    private void OnGattValueChanged(GattCharacteristic characteristic, GattValueChangedEventArgs args)
    {
        if (MessageDeserializer.TryDeserialize(args.CharacteristicValue.ToArray(), out var message))
        {
            OnReceived?.Invoke(message);
        }
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