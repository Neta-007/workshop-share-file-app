using Android.Net.Wifi.P2p;
using FileShareConnectivity.Platforms.Android.IO;
using FileShareConnectivity.Interfaces;
using Microsoft.Extensions.Logging;
using Java.Net;
using Javax.Net.Ssl;

namespace FileShareConnectivity.Platforms;

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
            _logger.LogDebug($"Invalid socket creation action.");
            return;
        }

        Task.Run(() =>
        {
            BaseFileSocket socket = null;

            try
            {
                _logger.LogDebug($"Create a socket and perform file transfer. filePath: {filePath}");
                socket = socketCreationAction();

                if (socket != null)
                {
                    if (filePath != null)
                    {
                        foreach (WifiP2pDevice device in _networkService._connectedPeers)
                        {
                            _logger.LogDebug($"InitiateFileTransfer. Sending file to: {device}");
                            socket.SendFile(filePath);
                        }
                    }
                    else
                    {
                        socket.ReceiveFileToSaveInDifferentApp();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in file transfer: {e}");
            }
            finally
            {
                _logger.LogDebug($"Finally block, dispose socket");
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

    /*private void startServerSocket(string filePath)
    {
        _logger.LogDebug($"Create a ServerSocket and listen on a specific port {SocketConfiguration.SocketPort}. filePath: {filePath}");

        if (_serverSocket == null)
        {
            _logger.LogDebug($"startServerSocket. Create new ServerSocketFile");
            _serverSocket = new ServerSocketFile();
        }

        Task.Run(() =>
        {
            try
            {
                _logger.LogDebug($"In Task.Run- startServerSocket. Start _serverSocket");
                _serverSocket.Start();

                if (filePath != null)
                {
                    foreach (WifiP2pDevice device in _networkService._connectedPeers)
                    {
                        _logger.LogDebug($"StartFileTransfer. sending file to: {device}");
                        _serverSocket.SendFile(filePath);
                    }
                }
                else
                {
                    _serverSocket.ReceiveFileToSaveInDifferentApp();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"In Task.Run- startServerSocket. Error: {e}");
            }
            finally
            {
                _logger.LogDebug($"In Task.Run- startServerSocket. finally block, dispose serverSocket");
                _serverSocket?.Dispose();
                _serverSocket = null;
            }
        });
    }

    private void connectToServerSocket(InetAddress serverAddress, string filePath)
    {
        _logger.LogDebug($"connectToServerSocket. filePath: {filePath}, serverAddress: {serverAddress}");
        // If the device is a client
        if (_clientSocket == null)
        {
            _logger.LogDebug($"connectToServerSocket. Create new ClientSocketFile");
            _clientSocket = new ClientSocketFile();
        }

        Task.Run(() =>
        {
            try
            {
                _logger.LogDebug($"In Task.Run- connectToServerSocket. Connect to host {serverAddress.HostAddress}");
                _clientSocket.Connect(serverAddress.HostAddress);

                if (filePath != null)
                {
                    foreach (WifiP2pDevice device in _networkService._connectedPeers)
                    {
                        _logger.LogDebug($"StartFileTransfer. sending file to: {device}");
                        _clientSocket.SendFile(filePath);
                    }
                }
                else
                {
                    _clientSocket.ReceiveFileToSaveInDifferentApp();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"In Task.Run- connectToServerSocket. Error {e}");
            }
            finally
            {
                _logger.LogDebug($"In Task.Run- connectToServerSocket. finally block, dispose clientSocket");
                _clientSocket?.Dispose();
                _clientSocket = null;
            }
        });
    }*/
}