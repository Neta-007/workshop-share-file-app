using Java.Net;
using Microsoft.Extensions.Logging;
using IOException = Java.IO.IOException;

namespace FileShareConnectivity.Platforms.Android.IO;

internal class ServerSocketFile : BaseFileSocket
{
    private ServerSocket _serverSocket;

    public ServerSocketFile() : base()
    {
        _logger = MauiApplication.Current.Services.GetService<ILogger<ServerSocketFile>>();
    }

    public void Start()
    {
        try
        {
            _logger.LogInformation($"Create a ServerSocket and listen on a specific port {SocketConfiguration.SocketPort}");
            _serverSocket = new ServerSocket(SocketConfiguration.SocketPort);

            // Accept incoming connections (reference of the client socket)
            _socket = _serverSocket.Accept();
            _logger.LogInformation($"ServerSocketFile Accept incoming connections (reference of the client socket)");

            // Create SendReceiveStreams to send/receive data
            _sendReceiveStreams = new SendReceiveStreamsFile(_socket);

            // Clean up
            // _socket.Close(); // Commented out to keep the server socket open for accepting new connections
            // _serverSocket.Close(); // Commented out to keep the server socket open for accepting new connections

        }
        catch (IOException e)
        {
            _logger.LogError($"Failed to accept client connection: {e.Message}");
        }
    }

    public override void Close()
    {
        try
        {
            base.Close();

            if (_serverSocket != null)
            {
                _logger.LogInformation($"ServerSocketFile closing _serverSocket. Port: {SocketConfiguration.SocketPort}");
                _serverSocket.Close();
                _serverSocket = null;
            }
        }
        catch (IOException e)
        {
            _logger.LogError("Failed to close sockets: " + e.Message);
        }
    }

    public override void Dispose()
    {
        Close();
    }
}
