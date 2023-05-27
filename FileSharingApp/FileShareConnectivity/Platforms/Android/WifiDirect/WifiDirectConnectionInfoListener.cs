using Android.Net.Wifi.P2p;
using Android.Util;
using FileShareConnectivity.EventArguments;
using FileShareConnectivity.Platforms.Android.io;
using Java.Net;
using static Android.Icu.Text.IDNA;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

public class WifiDirectConnectionInfoListener : Java.Lang.Object, WifiP2pManager.IConnectionInfoListener
{
    public event EventHandler<ConnectionResultEventArgs> ConnectionResult;

    public void OnConnectionInfoAvailable(WifiP2pInfo info)
    {
        if (info.GroupFormed && info.IsGroupOwner) 
        {
            // Device is the group owner (server side)
            // Start a server socket and listen for incoming connections
            StartServerSocket();
        } 
        else if (info.GroupFormed) 
        {
            // Device is the client side
            // Connect to the group owner's IP address
            ConnectToServerSocket(info.GroupOwnerAddress);
        }

        OnConnectionResult(new ConnectionResultEventArgs(info.GroupFormed, info));
    }

    public void StartServerSocket()
    {
        // Create a ServerSocket and listen on a specific port (SocketConfiguration.SocketPort)
        Task.Run(() =>
        {
            ServerSocketFile serverSocket = new ServerSocketFile();
            serverSocket.Start();
        });
    }

    public void ConnectToServerSocket(InetAddress serverAddress)
    {
        // If the device is a client
        Task.Run(() =>
        {
            ClientSocketFile clientSocket = new ClientSocketFile(serverAddress.HostAddress);
            clientSocket.Connect();
        });
    }

    protected virtual void OnConnectionResult(ConnectionResultEventArgs e)
    {
        if (ConnectionResult != null)
        {
            ConnectionResult(this, e);
        }
    }
}