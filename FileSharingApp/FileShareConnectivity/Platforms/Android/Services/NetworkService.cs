using System.Linq;
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.Net.Wifi.P2p;
using Android.Widget;
using FileShareConnectivity.EventArguments;
using FileShareConnectivity.Interfaces;
using FileShareConnectivity.Platforms.Android.enums;
using FileShareConnectivity.Platforms.Android.WifiDirect;

namespace FileShareConnectivity.Platforms;

internal class NetworkService : Java.Lang.Object, WifiP2pManager.IChannelListener, INetworkService
{
    private WifiP2pManager _wifiP2pManager;
    private WifiP2pManager.Channel _channel;
    private WifiDirectBroadcastReceiver _receiver;
    private IntentFilter _intentFilter;
    private Context _context;
    private LinkedList<WifiP2pDevice> _peerList = new();
    private LinkedList<Models.NearbyDevice> _nearbyDevicesName = new();
    private bool _initComponents = false;
    private bool _isReceiverRegistered = false;
    private ScanState _scanState = ScanState.None;

    public event EventHandler<DevicesEventArgs> DevicesFound;
    public event EventHandler<EventArgs> FinishScan;
    public event EventHandler<ConnectionResultEventArgs> ConnectionCompleted;

    public NetworkService()
    {
    }

    public void InitNetworkServiceComponents(Context context)
    {
        if (_initComponents)
        {
            return;
        }

        _initComponents = true;

        /*
         * _context = global::Android.App.Application.Context;
         * Application.Context generally should only be used if you need a Context whose lifecycle is separate from the current context,
         * that is tied to the lifetime of the process rather than the current component.
         * So, we will use the activity context
         */
        _context = context;

        // This class provide the API for managing Wi-Fi peer-to-peer connectivity
        _wifiP2pManager = (WifiP2pManager)_context.GetSystemService(Context.WifiP2pService);

        // Registers the application with the Wi-Fi framework. Call this before calling any other Wi-Fi P2P method.
        // A cannel that connects the application to the Wi-Fi p2p framework.
        // Most p2p operations require a Channel as an argument
        _channel = _wifiP2pManager.Initialize(_context, _context.MainLooper, null);

        // Create a BroadcastReceiver for receiving WifiP2pManager intents
        // With WifiDirectBroadcastReceiver & IntentFilter we can register and unregister the receiver
        // Also, register to DevicesDiscovered event
        _receiver = new WifiDirectBroadcastReceiver(_wifiP2pManager, _channel, _context);
        _receiver.DevicesDiscovered += receiver_DevicesDiscovered;
        _receiver.FinishDiscovery += receiver_FinishDiscovery;
        _receiver.ConnectionResult += receiver_ConnectionResult;

        _intentFilter = new IntentFilter();
        _intentFilter.AddAction(WifiP2pManager.WifiP2pStateChangedAction);
        _intentFilter.AddAction(WifiP2pManager.WifiP2pPeersChangedAction);
        _intentFilter.AddAction(WifiP2pManager.WifiP2pConnectionChangedAction);
        _intentFilter.AddAction(WifiP2pManager.WifiP2pThisDeviceChangedAction);
        _intentFilter.AddAction(WifiP2pManager.WifiP2pDiscoveryChangedAction);
    }

    public void StartDiscoverNearbyDevices()
    {
        _wifiP2pManager.DiscoverPeers(_channel, new WifiDirectActionListener(
            () => scanOperationCompleted(ScanState.Started, "Discovery started successfully."),
            (e) => scanOperationCompleted(ScanState.Failed, $"Failed to start discovery. Reason: {e}")));      // When it will find Peers => it will raise the intent in the _reciver
    }

    public void StopDiscoverNearbyDevices()
    {
        _wifiP2pManager.StopPeerDiscovery(_channel, new WifiDirectActionListener(
            () => scanOperationCompleted(ScanState.Stopped, "Discovery stopped successfully."),
            (e) => scanOperationCompleted(ScanState.Failed, $"Failed to stop discovery. Reason: {e}")));
    }

    public IEnumerable<Models.NearbyDevice> DiscoverNearbyDevices()
    {
        throw new NotImplementedException();
    }

    public void EstablishConnection(Models.NearbyDevice device)
    {
        WifiP2pDevice foundDevice = _peerList.FirstOrDefault(d => d.DeviceAddress == device.Address);

        if (foundDevice != null)
        {
            WifiP2pConfig config = new WifiP2pConfig
            {
                DeviceAddress = foundDevice.DeviceAddress,
                Wps =
                {
                    Setup = WpsInfo.Pbc
                }
            };
            
            _wifiP2pManager.Connect(_channel, config, new WifiDirectActionListener(
                () => showToastMessage("Start establishing connection"),
                e => showToastMessage($"Failed to start connection. Reason: {e}")));
        }
        else
        {
            //TODO 
        }

        //return true; //TODO: change to enum (maybe) that represents the establishment of the connection status
    }

    public void Disconnect()
    {
        _wifiP2pManager.RemoveGroup(_channel, new WifiDirectActionListener(
                () => showToastMessage("Disconnect"),
                e => showToastMessage($"Failed to disconnect. Reason: {e}")));
    }

    void WifiP2pManager.IChannelListener.OnChannelDisconnected()
    {
        // we will try once more
        /*if (_wifiP2pManager != null && !_retryChannel)
        {
            Toast.MakeText(_context, "Channel lost. Trying again", ToastLength.Long).Show();
            OnFinishScan(null);
            _retryChannel = true;
            _wifiP2pManager.Initialize(_context, _context.MainLooper, null);
        }
        else
        {
            Toast.MakeText(_context, "Severe! Channel is probably lost permanently. Try Disable/Re-Enable P2P.",
                           ToastLength.Long).Show();
        }*/
    }

    private void scanOperationCompleted(ScanState scanState, string message)
    {
        showToastMessage(message);
        _scanState = scanState;
    }

    private void showToastMessage(string message)
    {
        Toast.MakeText(_context, message, ToastLength.Short).Show();
    }

    /*
     * When WifiDirectActionListener finds devices => loop over devices in order to cast the object into a Model.Device object
     * for sending this object "outside" on the platform code
     */
    private void receiver_DevicesDiscovered(object sender, IEnumerable<WifiP2pDevice> deviceList)
    {
        _peerList.Clear();
        _nearbyDevicesName.Clear();
        foreach (WifiP2pDevice device in deviceList)
        {
            _peerList.AddLast(device);
            _nearbyDevicesName.AddLast(new Models.NearbyDevice(device.DeviceName, device.DeviceAddress));
        }

        if (_peerList.Count == 0)
        {
            showToastMessage("No devices found");
        }

        OnDeviceFound(new DevicesEventArgs(_nearbyDevicesName));
    }

    private void receiver_FinishDiscovery(object sender, EventArgs e)
    {
        if(_scanState != ScanState.None)       // TODO: when the app init the WifiP2pManager.WifiP2pDiscoveryChangedAction intent is triggered...
        {
            _scanState = _scanState == ScanState.Started ? ScanState.Stopped : _scanState;
            OnFinishScan(e);
        }
    }

    private void receiver_ConnectionResult(object sender, ConnectionResultEventArgs e)
    {
        OnConnectionCompleted(e);
    }

    public void RegisterReceiver()
    {
        if (_receiver != null && !_isReceiverRegistered)
        {
            _context.RegisterReceiver(_receiver, _intentFilter);
            _isReceiverRegistered = true;
        }
    }

    public void UnregisterReceiver()
    {
        if (_receiver != null && _isReceiverRegistered)
        {
            _context.UnregisterReceiver(_receiver);
            _isReceiverRegistered = false;
        }
    }

    protected virtual void OnDeviceFound(DevicesEventArgs e)
    {
        if (DevicesFound != null)
        {
            DevicesFound(this, e);
        }
    }

    protected virtual void OnFinishScan(EventArgs e)
    {
        if (FinishScan != null)
        {
            FinishScan(this, e);
        }
    }

    protected virtual void OnConnectionCompleted(ConnectionResultEventArgs e)
    {
        if (ConnectionCompleted != null)
        {
            ConnectionCompleted(this, e);
        }
    }
}
