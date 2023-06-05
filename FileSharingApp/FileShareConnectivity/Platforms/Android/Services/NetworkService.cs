using Android.Content;
using Android.Net.Wifi;
using Android.Net.Wifi.P2p;
using Android.Widget;
using FileShareConnectivity.enums;
using FileShareConnectivity.EventArguments;
using FileShareConnectivity.Exceptions;
using FileShareConnectivity.Interfaces;
using FileShareConnectivity.Platforms.Android.WifiDirect;
using Microsoft.Extensions.Logging;

namespace FileShareConnectivity.Platforms;

/*
 * TODO: 
 *  - Change all showToastMessage occurrences to throw the corresponding event for the action, and add log messages with the current toasted message.
 *  - Change the use of validateThatWifiP2PEnabled method. Use the intent WifiP2pStateChangedAction from the WifiDirectBroadcastReceiver
 */
internal class NetworkService : Java.Lang.Object, WifiP2pManager.IChannelListener, INetworkService
{
    private ILogger<NetworkService> _logger = MauiApplication.Current.Services.GetService<ILogger<NetworkService>>();
    private Context _context = global::Android.App.Application.Context;
    private WifiP2pManager _wifiP2pManager;
    private WifiP2pManager.Channel _channel;
    private WifiDirectBroadcastReceiver _receiver;
    private IntentFilter _intentFilter;
    private LinkedList<WifiP2pDevice> _peerList = new();
    private LinkedList<Models.NearbyDevice> _nearbyDevicesName = new();
    private bool _retryChannel = false;
    private bool _isReceiverRegistered = false;
    private ScanState _scanState = ScanState.None;
    private bool _isFirstFalseScanStart = true;

    internal LinkedList<WifiP2pDevice> _connectedPeers { private set; get; } = new();

    public event EventHandler<DevicesEventArgs> DevicesFound;
    public event EventHandler<ScanStateEventArgs> ScanStateChanged;
    public event EventHandler<ConnectionResultEventArgs> ConnectionResult;

    public NetworkService()
    {
        initNetworkServiceComponents();
    }

    private void initNetworkServiceComponents()
    {
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
        _receiver.DiscoveryChanged += receiver_DiscoveryChanged;
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
        _isFirstFalseScanStart = false;
        validateThatWifiP2PEnabled();
        _wifiP2pManager.DiscoverPeers(_channel, new WifiDirectActionListener(
            () => scanOperationCompleted(ScanState.Started, "Discovery started successfully."),
            (e) => scanOperationCompleted(ScanState.Failed, $"Failed to start discovery. Reason: {e}")));
    }

    public void StopDiscoverNearbyDevices()
    {
        StopDiscoverNearbyDevices(ScanState.Stopped);
    }

    public void StopDiscoverNearbyDevices(ScanState scanStateOnSuccess)
    {
        _wifiP2pManager.StopPeerDiscovery(_channel, new WifiDirectActionListener(
            () => updateScanState(scanStateOnSuccess),
            (e) => updateScanState(ScanState.Failed)));
    }

    public IEnumerable<Models.NearbyDevice> DiscoverNearbyDevices()
    {
        validateThatWifiP2PEnabled();
        throw new NotImplementedException();
    }

    public void EstablishConnection(Models.NearbyDevice device)
    {
        validateThatWifiP2PEnabled();
        WifiP2pDevice connectToPeer = _peerList.FirstOrDefault(d => d.DeviceAddress == device.Address);

        if (connectToPeer != null)
        {
            WifiP2pConfig config = new WifiP2pConfig();
            config.DeviceAddress = connectToPeer.DeviceAddress;
            config.Wps.Setup = WpsInfo.Pbc;
            
            _logger.LogDebug($"EstablishConnection. Start connecting with peer: {device}");
            _wifiP2pManager.Connect(_channel, config, new WifiDirectActionListener(
                () => 
                { 
                    showToastMessage("Start establishing connection");
                    _logger.LogDebug($"Start establishing connection");
                },
                e => 
                {
                    showToastMessage($"Failed to start connection. Reason: {e}");
                    _logger.LogError($"Failed to start connection. Reason: {e}");
                }));
        }
        else
        {
            throw new MyException("Fail to find the selected device", "Refresh discovery");
        }
    }

    public void Disconnect()
    {
        _wifiP2pManager.RemoveGroup(_channel, new WifiDirectActionListener(() => { }, e => { }));
    }

    void WifiP2pManager.IChannelListener.OnChannelDisconnected()
    {
        _logger.LogInformation($"Channel disconnected... We will try to connect once more");
        if (_wifiP2pManager != null && !_retryChannel)
        {
            showToastMessage("Channel lost. Trying again");
            updateScanState(ScanState.Failed);
            _retryChannel = true;
            _wifiP2pManager.Initialize(_context, _context.MainLooper, null);
        }
        else
        {
            _logger.LogError($"Severe! Channel is probably lost permanently. Try Disable/Re-Enable P2P.");
            showToastMessage("Severe! Channel is probably lost permanently. Try Disable/Re-Enable P2P.");
        }
    }

    private void receiver_DevicesDiscovered(object sender, IEnumerable<WifiP2pDevice> deviceList)
    {
        // When `WifiDirectActionListener` finds devices,
        // loop over the devices to cast the object into a `Model.Device` object,
        // in order to send this object "outside" the platform code.

        _peerList.Clear();
        _nearbyDevicesName.Clear();
        _connectedPeers.Clear();
        foreach (WifiP2pDevice device in deviceList)
        {
            _peerList.AddLast(device);
            _nearbyDevicesName.AddLast(new Models.NearbyDevice(device.DeviceName, device.DeviceAddress, device.Status == WifiP2pDeviceState.Connected));
            if(device.Status == WifiP2pDeviceState.Connected)
            {
                _connectedPeers.AddLast(device);
            }
        }

        _logger.LogDebug($"receiver_DevicesDiscovered. Number of found devices: {_nearbyDevicesName.Count}");
        _logger.LogDebug($"receiver_DevicesDiscovered. Number of connected devices: {_connectedPeers.Count}");

        OnDevicesFound(new DevicesEventArgs(_nearbyDevicesName));
    }

    private void receiver_DiscoveryChanged(object sender, ScanStateEventArgs e)
    {
        if(_scanState != ScanState.None)       // TODO ?: when the app init the WifiP2pManager.WifiP2pDiscoveryChangedAction intent is triggered...
        {
            updateScanState(e.eScanState);
        }
    }

    private void receiver_ConnectionResult(object sender, ConnectionResultEventArgs e)
    {
        // _logger.LogDebug($"receiver_ConnectionResult. IsSuccessConnection {e.IsSuccessConnection}, ConnectionInfo {e.ConnectionInfo}");

        OnConnectionResult(e);
    }

    private void updateScanState(ScanState scanState)
    {
        // Logically, ScanState.Stopped can only occur after ScanState.Started.
        // Too many unnecessary scan events are being triggered due to the intent in the receiver.
        // Sometimes, the first thrown ScanState.Started is false due to the intent in the receiver.

        bool scanBaseLogic = _scanState != scanState;
        bool scanFirstStartLogic = scanState != ScanState.Started || (scanState == ScanState.Started && !_isFirstFalseScanStart);
        bool scanStopLogic = scanState != ScanState.Stopped || (scanState == ScanState.Stopped && _scanState != ScanState.Reset);

        if (scanBaseLogic && scanFirstStartLogic && scanStopLogic)
        {
            _scanState = scanState;
            OnScanStateChanged(new ScanStateEventArgs(_scanState));
        }
    }

    private void scanOperationCompleted(ScanState scanState, string message)
    {
        showToastMessage(message);
        updateScanState(scanState);
        _logger.LogDebug($"scanOperationCompleted. scanState: {scanState}, message: {message}");
    }

    private void showToastMessage(string message)   // TODO: Remove this
    {
        Toast.MakeText(_context, message, ToastLength.Short).Show();
        _logger.LogDebug($"showToastMessage. message: {message}");
    }

    private void validateThatWifiP2PEnabled()   // TODO: Remove this
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

    protected virtual void OnDevicesFound(DevicesEventArgs e)
    {
        if (DevicesFound != null)
        {
            DevicesFound(this, e);
        }
    }

    protected virtual void OnScanStateChanged(ScanStateEventArgs e)
    {
        if (ScanStateChanged != null)
        {
            ScanStateChanged(this, e);
        }
    }

    protected virtual void OnConnectionResult(ConnectionResultEventArgs e)
    {
        if (ConnectionResult != null)
        {
            ConnectionResult(this, e);
        }
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

    internal void CleanAnyOpenConnections()
    {
        UnregisterReceiver();
        StopDiscoverNearbyDevices(ScanState.Reset);
        Disconnect();
    }
}
