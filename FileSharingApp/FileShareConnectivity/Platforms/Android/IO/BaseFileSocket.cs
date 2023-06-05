using Java.Net;
using Microsoft.Extensions.Logging;
using IOException = Java.IO.IOException;

namespace FileShareConnectivity.Platforms.Android.IO;

internal abstract class BaseFileSocket : IDisposable
{
    protected ILogger<BaseFileSocket> _logger;
    protected SendReceiveStreamsFile _sendReceiveStreams;
    protected Socket _socket;

    public virtual void SendFile(string filePath)
    {
        if (_sendReceiveStreams != null)
        {
            _sendReceiveStreams.SendFile(filePath);
        }
    }

    public virtual void ReceiveFile()
    {
        if (_sendReceiveStreams != null)
        {
            _sendReceiveStreams.ReceiveFile();
        }
    }

    public virtual void Close()
    {
        try
        {
            if (_socket != null)
            {
                _logger.LogInformation($"Closing socket. Port: {SocketConfiguration.SocketPort}");
                _socket.Close();
                _socket = null;
            }
        }
        catch (IOException e)
        {
            _logger.LogError("Failed to close sockets: " + e.Message);
        }
    }

    public abstract void Dispose();
}
