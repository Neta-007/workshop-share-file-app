using Android.Net.Wifi.P2p;
using FileShareConnectivity.Platforms.Android.IO;
using FileShareConnectivity.Interfaces;
using Microsoft.Extensions.Logging;
using Java.Net;

namespace FileShareConnectivity.Platforms;
/*
 * TODO: For now, the current logic is that the device holding the file path to send is the one responsible for sending it, regardless of who initiated the connection. 
 *       The reason for implementing it this way is to enable both devices to send and receive files. 
 *       However, there is a need to close this process in a more hermetic manner.
 */
internal class FileTransferService : IFileTransferService
{
    private ILogger<FileTransferService> _logger = MauiApplication.Current.Services.GetService<ILogger<FileTransferService>>();
    private NetworkService _networkService = (NetworkService)MauiApplication.Current.Services.GetService(typeof(INetworkService));
    private ServerSocketFile _serverSocket;
    private ClientSocketFile _clientSocket;

    public void StartFileTransfer(object connectionInfo, string filePath)
    {
        if (connectionInfo != null)
        {
            WifiP2pInfo info = connectionInfo as WifiP2pInfo;

            _logger.LogDebug($"StartFileTransfer. filePath: {filePath}, connectionInfo: {info}");
            if (info != null)
            {
                if (info.GroupFormed && info.IsGroupOwner)
                {
                    startServerSocket(filePath);
                }
                else if (info.GroupFormed)
                {
                    connectToServerSocket(info.GroupOwnerAddress, filePath);
                }
            }
        }
    }

    private void initiateFileTransfer(string filePath, Func<BaseFileSocket> socketCreationAction, bool isServerSocket)
    {
        _logger.LogDebug($"InitiateFileTransfer. filePath: {filePath}");

        if (socketCreationAction == null)
        {
            _logger.LogError($"Invalid socket creation action. Exit initiateFileTransfer operation");
            return;
        }

        Task.Run(() =>
        {
            BaseFileSocket socket = null;

            try
            {
                _logger.LogDebug($"Creating a socket and perform file transfer. filePath: {filePath}");
                socket = socketCreationAction();

                if (socket != null)
                {
                    if (filePath != null)
                    {
                        _logger.LogDebug($"InitiateFileTransfer. Sending file to {_networkService._connectedPeers.Count} peers");
                        foreach (WifiP2pDevice device in _networkService._connectedPeers)
                        {
                            _logger.LogDebug($"InitiateFileTransfer. Sending file to: {device}");
                            socket.SendFile(filePath);
                        }
                    }
                    else
                    {
                        socket.ReceiveFile();
                    }
                }
                else
                {
                    _logger.LogError($"Error creating socket in file transfer. socket is null");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in file transfer: {e}");
            }
            finally
            {
                _logger.LogDebug($"Finally block in file transfer, dispose socket");
                socket?.Dispose();
                if (isServerSocket)
                {
                    _serverSocket = null;
                }
                else
                {
                    _clientSocket = null;
                }
            }
        });
    }

    private void startServerSocket(string filePath)
    {
        initiateFileTransfer(filePath, () =>
        {
            if (_serverSocket == null)
            {
                _logger.LogDebug($"Create new ServerSocketFile");
                _serverSocket = new ServerSocketFile();
            }

            _logger.LogDebug($"Start _serverSocket");
            _serverSocket.Start();

            return _serverSocket;
        }, true);
    }

    private void connectToServerSocket(InetAddress serverAddress, string filePath)
    {
        initiateFileTransfer(filePath, () =>
        {
            if (_clientSocket == null)
            {
                _logger.LogDebug($"Create new ClientSocketFile");
                _clientSocket = new ClientSocketFile();
            }

            _logger.LogDebug($"Connect to host {serverAddress.HostAddress}");
            _clientSocket.Connect(serverAddress.HostAddress);

            return _clientSocket;
        }, false);
    }
}