
using FileShareConnectivity.EventArguments;

namespace FileShareConnectivity.Interfaces;

public interface INetworkService
{
    public event EventHandler<DevicesEventArgs> DevicesFound;
    public event EventHandler<EventArgs> FinishScan;

    void StartDiscoverNearbyDevices();
    IEnumerable<Models.NearbyDevice> DiscoverNearbyDevices();
    bool EstablishConnection(Models.NearbyDevice device);
    bool DisconnectFromDevice(Models.NearbyDevice device);
}
