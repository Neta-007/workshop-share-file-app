using Android.Content;
using FileShareConnectivity.EventArguments;
using FileShareConnectivity.Exceptions;
using FileShareConnectivity.Interfaces;
using FileShareConnectivity.Models;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

internal class NetworkProxy : INetworkService
{
    private NetworkService _networkService;

    public NetworkProxy(Context context)
    {
        _networkService = (NetworkService)MauiApplication.Current.Services.GetService(typeof(INetworkService));
        _networkService.InitNetworkServiceComponents(context);
        RegisterReceiver();
    }

    public event EventHandler<DevicesEventArgs> DevicesFound
    {
        add { _networkService.DevicesFound += value; }
        remove { _networkService.DevicesFound -= value; }
    }

    public event EventHandler<ScanStateEventArgs> ScanStateChanged
    {
        add { _networkService.ScanStateChanged += value; }
        remove { _networkService.ScanStateChanged -= value; }
    }

    public event EventHandler<ConnectionResultEventArgs> ConnectionResult
    {
        add { _networkService.ConnectionResult += value; }
        remove { _networkService.ConnectionResult -= value; }
    }

    public void RegisterReceiver()
    {
        _networkService.RegisterReceiver();
    }

    public void UnregisterReceiver()
    {
        _networkService.UnregisterReceiver();
    }

    public void StartDiscoverNearbyDevices()
    {
        validateThatWifiP2PEnabled();
        _networkService.StartDiscoverNearbyDevices();
    }

    public IEnumerable<NearbyDevice> DiscoverNearbyDevices()
    {
        validateThatWifiP2PEnabled();
        return _networkService.DiscoverNearbyDevices();
    }

    public void EstablishConnection(NearbyDevice device)
    {
        validateThatWifiP2PEnabled();
        _networkService.EstablishConnection(device);
    }

    public void Disconnect()
    {
        // validateThatWifiP2PEnabled(); ????
        _networkService.Disconnect();
    }

    private void validateThatWifiP2PEnabled()
    {
        if (!Utils.IsWifiEnabled())
        {
            throw new MyException("Please enable Wi-Fi", "");
        }
        else if (!Utils.IsWifiDirectSupported())
        {
            throw new MyException("Wi-Fi Direct is not supported on this device", "");
        }
    }

    public void CleanAnyConnections()
    {
        _networkService.UnregisterReceiver();
        _networkService.StopDiscoverNearbyDevices();
        _networkService.Disconnect();
    }
}
