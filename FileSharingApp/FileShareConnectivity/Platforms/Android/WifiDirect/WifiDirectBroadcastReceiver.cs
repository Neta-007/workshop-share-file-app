using Android.Content;
using Android.Net;
using Android.Net.Wifi.P2p;
using FileShareConnectivity.enums;
using FileShareConnectivity.EventArguments;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

/*
 * A BroadcastReceiver that notifies of important wifi p2p events using 4 intents.
 * https://developer.android.com/guide/topics/connectivity/wifip2p --> Table 3. Wi-Fi P2P Intents
*/
internal class WifiDirectBroadcastReceiver : BroadcastReceiver
{
    private readonly Context _context;
    private readonly WifiP2pManager _manager;
    private readonly WifiP2pManager.Channel _channel;
    private readonly WifiDirectPeerListListener _peerListListener; 

    public event EventHandler<IEnumerable<WifiP2pDevice>> DevicesDiscovered;
    public event EventHandler<ConnectionResultEventArgs> ConnectionResult;

    public event EventHandler<EventArgs> StateChanged;
    public event EventHandler<EventArgs> ThisDeviceChanged;
    public event EventHandler<ScanStateEventArgs> DiscoveryChanged;

    public WifiDirectBroadcastReceiver(WifiP2pManager manager, WifiP2pManager.Channel channel, Context context)
    {
        _context = context;
        _manager = manager;
        _channel = channel;
        _peerListListener = new WifiDirectPeerListListener();
        _peerListListener.DevicesDiscovered += peerListListener_DevicesDiscovered;
    }

    public override void OnReceive(Context context, global::Android.Content.Intent intent)
    {
        string action = intent.Action;

        switch (action)
        {
            case WifiP2pManager.WifiP2pStateChangedAction:
                handleWifiP2pStateChangedAction(intent);
                break;
            case WifiP2pManager.WifiP2pPeersChangedAction:
                handleWifiP2pPeersChangedAction();
                break;
            case WifiP2pManager.WifiP2pConnectionChangedAction:
                handleWifiP2pConnectionChangedAction(intent);
                break;
            case WifiP2pManager.WifiP2pThisDeviceChangedAction:
                handleWifiP2pThisDeviceChangedAction(intent);
                break;
            case WifiP2pManager.WifiP2pDiscoveryChangedAction:
                handleWifiP2pDiscoveryChangedAction(intent);
                break;
        }
    }

    private void handleWifiP2pStateChangedAction(global::Android.Content.Intent intent)
    {
        OnStateChanged(new EventArgs());
    }

    private void handleWifiP2pPeersChangedAction()
    {
        // Broadcast when you call discoverPeers().
        // You will usually call requestPeers() to get an updated list of peers if you handle this intent in your application.
        // Call WifiP2pManager.requestPeers() to get a list of current peers

        // request available peers from the wifi p2p manager. This is an
        // asynchronous call and the calling activity is notified with a
        // callback on PeerListListener.onPeersAvailable()

        // Request a list of available peers from the WifiP2pManager and raise the DevicesDiscovered event with the results.
        _manager.RequestPeers(_channel, _peerListListener);
    }

    private void handleWifiP2pConnectionChangedAction(global::Android.Content.Intent intent)
    {
        bool isNetworkConntected = isP2PNetworkConntectedToDevice();

        if (isNetworkConntected)
        {
            // we are connected with the other device, request connection info to find group owner IP
            WifiDirectConnectionInfoListener wifiDirectConnectionInfoListener = new WifiDirectConnectionInfoListener();
            wifiDirectConnectionInfoListener.ConnectionResult += WifiDirectConnectionInfoListener_ConnectionResult;
            _manager.RequestConnectionInfo(_channel, wifiDirectConnectionInfoListener);
        }
        else
        {
            // TODO: disconnected ?? => Can't connet?
            OnConnectionResult(new ConnectionResultEventArgs(false, null));
        }
    }

    private void handleWifiP2pThisDeviceChangedAction(global::Android.Content.Intent intent)
    {
        OnThisDeviceChanged(new EventArgs());
    }

    private void handleWifiP2pDiscoveryChangedAction(global::Android.Content.Intent intent)
    {
        // Note that discovery will be stopped during a connection setup.
        // If the application tries to re-initiate discovery during this time, it can fail.
        // Broadcast intent action indicating that peer discovery has either started or stopped.
        // https://developer.android.com/reference/android/net/wifi/p2p/WifiP2pManager#WIFI_P2P_DISCOVERY_CHANGED_ACTION
        int discoveryState = intent.GetIntExtra(WifiP2pManager.ExtraDiscoveryState, -1);
        ScanState newState = ScanState.Started;

        if (discoveryState == (int)WifiP2pManager.WifiP2pDiscoveryStopped)
        {
            newState = ScanState.Stopped;
        }

        OnDiscoveryChanged(new ScanStateEventArgs(newState));
    }

    private bool isP2PNetworkConntectedToDevice()
    {
        bool isNetworkConntected = false;
        ConnectivityManager connectivityManager = (ConnectivityManager)_context.GetSystemService(Context.ConnectivityService);

        if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.M)
        {
            // SDK supported 23(M) and above
            Network network = connectivityManager.ActiveNetwork;
            NetworkCapabilities activeNetwork = connectivityManager.GetNetworkCapabilities(network);
            isNetworkConntected = activeNetwork != null &&
                                  activeNetwork.HasCapability(global::Android.Net.NetCapability.Internet);
        }
        else
        {
            // SDK supported 21 - 29
            isNetworkConntected = connectivityManager.ActiveNetworkInfo.IsConnected;
        }

        return isNetworkConntected;
    }

    private void WifiDirectConnectionInfoListener_ConnectionResult(object sender, ConnectionResultEventArgs e)
    {
        OnConnectionResult(e);
    }

    private void peerListListener_DevicesDiscovered(object sender, IEnumerable<WifiP2pDevice> e)
    {
        OnDevicesDiscovered(e);
    }

    protected virtual void OnDevicesDiscovered(IEnumerable<WifiP2pDevice> devices)
    {
        if (DevicesDiscovered != null)
        {
            DevicesDiscovered(this, devices);
        }
    }

    protected virtual void OnConnectionResult(ConnectionResultEventArgs e)
    {
        if (ConnectionResult != null)
        {
            ConnectionResult(this, e);
        }
    }

    protected virtual void OnStateChanged(EventArgs e)
    {
        if (StateChanged != null)
        {
            StateChanged(this, e);
        }
    }

    protected virtual void OnThisDeviceChanged(EventArgs e)
    {
        if (ThisDeviceChanged != null)
        {
            ThisDeviceChanged(this, e);
        }
    }

    protected virtual void OnDiscoveryChanged(ScanStateEventArgs e)
    {
        if (DiscoveryChanged != null)
        {
            DiscoveryChanged(this, e);
        }
    }
}
