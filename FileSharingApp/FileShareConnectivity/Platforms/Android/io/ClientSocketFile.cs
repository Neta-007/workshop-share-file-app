using Java.Net;
using Microsoft.Extensions.Logging;

namespace FileShareConnectivity.Platforms.Android.IO;

internal class ClientSocketFile : BaseFileSocket
{
    private ILogger<ClientSocketFile> _logger = MauiApplication.Current.Services.GetService<ILogger<ClientSocketFile>>();
    private Socket _socket;

    public void Connect(string hostAddress)
    {
        try
        {
            _logger.LogInformation($"ClientSocketFile Initiate a connection to the socket server. HostAddress: {hostAddress}, Port: {SocketConfiguration.SocketPort}, timeout: {SocketConfiguration.SocketTimeout}");
            _socket = new Socket();
            _socket.Bind(null);
            _socket.Connect(new InetSocketAddress(hostAddress, SocketConfiguration.SocketPort), SocketConfiguration.SocketTimeout);

            // Create SendReceiveStreams to send/receive data
            _sendReceiveStreams = new SendReceiveStreamsFile(_socket);

            // Clean up
            // Close(); // Commented out to keep the socket open for sending files
        }
        catch (IOException e)
        {
            _logger.LogError($"Failed to connect to server: {e.Message}");
        }
    }

    public override void SendFile(string filePath)
    {
        if (_sendReceiveStreams != null)
        {
            _sendReceiveStreams.SendFile(filePath);
        }
    }

    public override void ReceiveFileToSaveInDifferentApp()
    {
        if (_sendReceiveStreams != null)
        {
            _sendReceiveStreams.ReceiveFile();//.ReceiveFileToSaveInDifferentApp();
        }
    }

    public override void Close()
    {
        try
        {
            if (_socket != null)
            {
                _logger.LogInformation($"ClientSocketFile closing socket. Port: {SocketConfiguration.SocketPort}, timeout: {SocketConfiguration.SocketTimeout}");
                _socket.Close();
                _socket = null;
            }
        }
        catch (IOException e)
        {
            _logger.LogError($"Failed to close socket: {e.Message}");
        }
    }

    public override void Dispose()
    {
        Close();
    }
}