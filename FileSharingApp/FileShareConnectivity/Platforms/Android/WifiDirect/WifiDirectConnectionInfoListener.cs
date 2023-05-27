using Android.Net.Wifi.P2p;
using FileShareConnectivity.EventArguments;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

public class WifiDirectConnectionInfoListener : Java.Lang.Object, WifiP2pManager.IConnectionInfoListener
{
    public event EventHandler<ConnectionResultEventArgs> ConnectionResult;

    public void OnConnectionInfoAvailable(WifiP2pInfo info)
    {
        //TODO:in case we are going to do groups implement this
        var groupOwnerAddress = info.GroupOwnerAddress?.HostAddress;    // We will use it in the client side when we send 

        if (info.GroupFormed && info.IsGroupOwner) {                    // This is the host
            // Do whatever tasks are specific to the group owner.
            // One common case is creating a group owner thread and accepting
            // incoming connections.
        } else if (info.GroupFormed) {                                  // this is the client
            // The other device acts as the peer (client). In this case,
            // you'll want to create a peer thread that connects
            // to the group owner.
        }

        OnConnectionResult(info);
    }

    protected virtual void OnConnectionResult(WifiP2pInfo info)
    {
        if (ConnectionResult != null)
        {
            ConnectionResult(this, new ConnectionResultEventArgs(true, info));
        }
    }
}