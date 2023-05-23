using Android.Net.Wifi.P2p;


namespace FileShareConnectivity.Platforms.Android.WifiDirect;

public class WifiDirectConnectionInfoListener: Java.Lang.Object, WifiP2pManager.IConnectionInfoListener
{
    public void OnConnectionInfoAvailable(WifiP2pInfo info)
    {
        //TODO:in case we are going to do groups implement this
        var groupOwnerAddress = info.GroupOwnerAddress?.HostAddress;
        
        if (info.GroupFormed && info.IsGroupOwner) {
            // Do whatever tasks are specific to the group owner.
            // One common case is creating a group owner thread and accepting
            // incoming connections.
        } else if (info.GroupFormed) {
            // The other device acts as the peer (client). In this case,
            // you'll want to create a peer thread that connects
            // to the group owner.
        }
    }
}