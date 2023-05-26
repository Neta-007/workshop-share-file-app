using System;
using System.IO;
using System.Threading.Tasks;
using Java.Net;
using Environment = global::Android.OS.Environment;
using File = Java.IO.File;
using IOException = Java.IO.IOException;

namespace FileShareConnectivity.Platforms.Android.WifiDirect;

public class FileServer
{
    private Socket _socket;
    private ServerSocket _serverSocket;

    public void Run()
    {
        try
        {
            _serverSocket = new ServerSocket(SocketConfiguration.SocketPort);
            _socket = _serverSocket.Accept(); //Now the socket will be ready to accept the file
        }
        catch (IOException ex)
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