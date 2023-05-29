using Android.Net.Wifi.P2p;
using FileShareConnectivity.EventArguments;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

public class WifiDirectConnectionInfoListener : Java.Lang.Object, WifiP2pManager.IConnectionInfoListener
{
    public event EventHandler<ConnectionResultEventArgs> ConnectionResult;

    public void OnConnectionInfoAvailable(WifiP2pInfo info)
    {
        OnConnectionResult(new ConnectionResultEventArgs(info.GroupFormed, info));
    }

    protected virtual void OnConnectionResult(ConnectionResultEventArgs e)
    {
        if (ConnectionResult != null)
        {
            ConnectionResult(this, e);
        }
    }
}