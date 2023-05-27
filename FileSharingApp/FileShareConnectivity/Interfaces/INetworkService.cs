
using FileShareConnectivity.EventArguments;

namespace FileShareConnectivity.Interfaces;

public interface INetworkService
{
    public event EventHandler<DevicesEventArgs> DevicesFound;
    public event EventHandler<ScanStateEventArgs> ScanStateChanged;
    public event EventHandler<ConnectionResultEventArgs> ConnectionResult;

    void StartDiscoverNearbyDevices();
    IEnumerable<Models.NearbyDevice> DiscoverNearbyDevices();
    void EstablishConnection(Models.NearbyDevice device);
    void Disconnect();
}
