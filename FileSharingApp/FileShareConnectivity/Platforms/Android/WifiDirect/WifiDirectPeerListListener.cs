using Android.Net.Wifi.P2p;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

/*
 * This listener is used to provide callbacks when the discovery process succeeds or fails.
 */
internal class WifiDirectPeerListListener : Java.Lang.Object, WifiP2pManager.IPeerListListener
{
    public event EventHandler<IEnumerable<WifiP2pDevice>> DevicesDiscovered;

    public void OnPeersAvailable(WifiP2pDeviceList peers)
    {
        OnDevicesDiscovered(peers.DeviceList);
    }

    protected virtual void OnDevicesDiscovered(IEnumerable<WifiP2pDevice> devices)
    {
        if (DevicesDiscovered != null)
        {
            DevicesDiscovered(this, devices);
        }
    }
}