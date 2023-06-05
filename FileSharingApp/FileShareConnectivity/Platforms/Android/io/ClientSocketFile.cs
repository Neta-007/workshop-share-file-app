using Java.Net;
using Microsoft.Extensions.Logging;

namespace FileShareConnectivity.Platforms.Android.IO;

internal class ClientSocketFile : BaseFileSocket
{
    public ClientSocketFile() : base()
    {
        _logger = MauiApplication.Current.Services.GetService<ILogger<ClientSocketFile>>();
    }

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

    public override void Dispose()
    {
        Close();
    }
}
