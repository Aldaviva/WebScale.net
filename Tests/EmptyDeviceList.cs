using HidSharp;

namespace Tests;

public class EmptyDeviceList: DeviceList {

    public static readonly EmptyDeviceList Instance = new();

    public override IEnumerable<Device> GetAllDevices() {
        return Enumerable.Empty<Device>();
    }

    public override IEnumerable<Device> GetDevices(DeviceTypes types) {
        return Enumerable.Empty<Device>();
    }

    public override bool AreDriversBeingInstalled { get; } = false;

}