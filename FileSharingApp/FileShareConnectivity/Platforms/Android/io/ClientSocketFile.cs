using Java.Net;

namespace FileShareConnectivity.Platforms.Android.io;

internal class ClientSocketFile : IDisposable
{
    private SendReceiveStreamsFile _sendReceiveStreams;
    private Socket _socket;
    private string _hostAddress;

    public ClientSocketFile(string hostAddress)
    {
        _hostAddress = hostAddress;
    }

    public void Connect()
    {
        try
        {
            _socket = new Socket();
            _socket.Bind(null);
            _socket.Connect(new InetSocketAddress(_hostAddress, SocketConfiguration.SocketPort), SocketConfiguration.SocketTimeout);

            // Create SendReceiveStreams to send/receive data
            _sendReceiveStreams = new SendReceiveStreamsFile(_socket);

            // Clean up
            // Close(); // Commented out to keep the socket open for sending files
        }
        catch (IOException e)
        {
            // "Failed to connect to server: " + e.Message
        }
    }

    public void SendFile(string filePath)
    {
        if (_sendReceiveStreams != null)
        {
            _sendReceiveStreams.SendFile(filePath);
        }
    }

    public void Close()
    {
        try
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }
        catch (IOException e)
        {
            // "Failed to close socket: " + e.Message
        }
    }

    public void Dispose()
    {
        Close();
    }
}