using System.Globalization;
using Java.IO;
using Java.Net;
using Console = System.Console;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

public class FileSendReceiveStreams
{
    private Socket _socket;
    private Stream _inputStream;
    private Stream _outputStream;

    public FileSendReceiveStreams(Socket socket)
    {
        _socket = socket;
        try
        {
            _inputStream = socket.InputStream;
            _outputStream = socket.OutputStream;
        }
        catch (Exception e)
        {
            //TODO
        }
        
    }
}