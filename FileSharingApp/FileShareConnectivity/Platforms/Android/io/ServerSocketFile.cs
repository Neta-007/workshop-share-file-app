using Java.Net;
using IOException = Java.IO.IOException;

namespace FileShareConnectivity.Platforms.Android.io;

internal class ServerSocketFile : IDisposable
{
    private SendReceiveStreamsFile _sendReceiveStreams;
    private ServerSocket _serverSocket;
    private Socket _clientSocket;

    public void Start()
    {
        try
        {
            _serverSocket = new ServerSocket(SocketConfiguration.SocketPort);

            // Accept incoming connections
            _clientSocket = _serverSocket.Accept();

            // Create SendReceiveStreams to send/receive data
            _sendReceiveStreams = new SendReceiveStreamsFile(_clientSocket);

            // Clean up
            // _clientSocket.Close(); // Commented out to keep the server socket open for accepting new connections
            // _serverSocket.Close(); // Commented out to keep the server socket open for accepting new connections

        }
        catch (IOException e)
        {
            // $"Failed to accept client connection: {e.Message}"
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
            if (_clientSocket != null)
            {
                _clientSocket.Close();
                _clientSocket = null;
            }

            if (_serverSocket != null)
            {
                _serverSocket.Close();
                _serverSocket = null;
            }
        }
        catch (IOException e)
        {
            // "Failed to close sockets: " + e.Message
        }
    }

    public void Dispose()
    {
        Close();
    }
}