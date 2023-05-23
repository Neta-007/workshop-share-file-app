
namespace FileShareConnectivity.EventArguments;

public class DevicesEventArgs : EventArgs
{
    public IEnumerable<Models.NearbyDevice> DeviceList { get; private set; }

    public bool AreDevicesFound { get; private set; }

    public string MessageWhenFail { get; private set; }

    public DevicesEventArgs(IEnumerable<Models.NearbyDevice> deviceList, string messageWhenFail = null)
    {
        DeviceList = deviceList;
        AreDevicesFound = DeviceList != null;
        MessageWhenFail = messageWhenFail;
    }
}
