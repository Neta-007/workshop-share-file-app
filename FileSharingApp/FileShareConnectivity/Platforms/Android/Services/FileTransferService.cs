using Android.Net.Wifi.P2p;
using FileShareConnectivity.Platforms.Android.IO;
using Java.Net;
using FileShareConnectivity.Interfaces;
using Android.Content;

namespace FileShareConnectivity.Platforms;

internal class FileTransferService : IFileTransferService
{
    private const int BufferSize = 1024;
    private ServerSocketFile _serverSocket;
    private ClientSocketFile _clientSocket;

    public Context Context { get; internal set; }

    public void StartFileTransfer(object connectionInfo, string filePath)
    {
        if (connectionInfo != null)
        {
            WifiP2pInfo info = connectionInfo as WifiP2pInfo;

            if (info != null)
            {
                if (info.GroupFormed && info.IsGroupOwner)
                {
                    // Device is the group owner (server side)
                    // Start a server socket and listen for incoming connections
                    // server (receiver), client (sender)?
                    startServerSocket(filePath);
                }
                else if (info.GroupFormed)
                {
                    // Device is the client side
                    // Connect to the group owner's IP address
                    connectToServerSocket(info.GroupOwnerAddress, filePath);
                }
            }
        }
        //throw new NotImplementedException();
    }

    private void startServerSocket(string filePath)
    {
        // Create a ServerSocket and listen on a specific port (SocketConfiguration.SocketPort)
        Task.Run(() =>
        {
            try
            {
                if (_serverSocket == null)
                {
                    _serverSocket = new ServerSocketFile();
                    _serverSocket.Start();

                    if (filePath != null)
                    {
                        _serverSocket.SendFile(filePath);
                    }
                    else
                    {
                        _serverSocket.ReceiveFileToSaveInDifferentApp(Context);
                    }
                }
            }
            catch(Exception e)
            {

            }
            finally
            {
                _serverSocket.Close();
            }
        });
    }

    private void connectToServerSocket(InetAddress serverAddress, string filePath)
    {
        // If the device is a client
        Task.Run(() =>
        {
            try
            {
                if (_clientSocket == null)
                {
                    _clientSocket = new ClientSocketFile();
                    _clientSocket.Connect(serverAddress.HostAddress);

                    if (filePath != null)
                    {
                        _clientSocket.SendFile(filePath);
                    }
                    else
                    {
                        _clientSocket.ReceiveFileToSaveInDifferentApp(Context);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                _clientSocket.Close();
            }
        });
    }
}