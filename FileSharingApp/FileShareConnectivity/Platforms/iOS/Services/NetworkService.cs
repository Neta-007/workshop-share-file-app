using FileShareConnectivity.EventArguments;
using FileShareConnectivity.Interfaces;

namespace FileShareConnectivity.Platforms;

internal class NetworkService : INetworkService
{
    public event EventHandler<DevicesEventArgs> DevicesFound;
    public event EventHandler<EventArgs> FinishScan;

    public void StartDiscoverNearbyDevices()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Models.NearbyDevice> DiscoverNearbyDevices()
    {
        throw new NotImplementedException();
    }

    public bool EstablishConnection(Models.NearbyDevice device)
    {
        throw new NotImplementedException();
    }

    public bool DisconnectFromDevice(Models.NearbyDevice device)
    {
        throw new NotImplementedException();
    }
}
