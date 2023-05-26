using Java.Net;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

public class FileClient
{
    private Socket _socket;
    private string _hostAddress;

    
    public FileClient(InetAddress hostAddress)
    {
        _hostAddress = hostAddress.HostAddress;
        _socket = new Socket();
    }

    public void Run()
    {
        try
        {
            _socket.Bind(null);
            _socket.Connect(new InetSocketAddress(_hostAddress, SocketConfiguration.SocketPort), SocketConfiguration.SocketPort);
        }
        catch (IOException e)
        {
            //TODO
        }
        finally
        {
            if (_socket != null && _socket.IsConnected)
            {
                try
                {
                    _socket.Close();
                }
                catch (Exception e)
                {
                    //TODO
                }
            }
        }
    }
}