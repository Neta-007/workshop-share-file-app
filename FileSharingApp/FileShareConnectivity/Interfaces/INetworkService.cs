
using FileShareConnectivity.EventArguments;

namespace FileShareConnectivity.Interfaces;

public interface INetworkService
{
    public event EventHandler<DevicesEventArgs> DevicesFound;
    public event EventHandler<EventArgs> FinishScan;
    public event EventHandler<ConnectionResultEventArgs> ConnectionCompleted;

    void StartDiscoverNearbyDevices();
    IEnumerable<Models.NearbyDevice> DiscoverNearbyDevices();
    void EstablishConnection(Models.NearbyDevice device);
    bool DisconnectFromDevice(Models.NearbyDevice device);
}
