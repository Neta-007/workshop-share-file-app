using System.IO;
using System.Net.Sockets;
using Android.Net.Wifi.P2p;
using FileShareConnectivity.Interfaces;

namespace FileShareConnectivity.Platforms;

internal class FileTransferService : IFileTransferService
{
    private const int BufferSize = 1024;

    public async Task<bool> SendFileAsync(object connectionInfo, string filePath)
    {
        if(connectionInfo != null && filePath != null)
        {
            WifiP2pInfo info = connectionInfo as WifiP2pInfo;

            if(info != null) 
            {
                if(info.GroupFormed && info.IsGroupOwner)
                {
                    byte[] data = ;

                    try
                    {
                        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                        {
                            await socket.ConnectAsync(_device.DeviceAddress, 8888);

                            using (var fileStream = File.OpenRead(filePath))
                            {
                                byte[] buffer = new byte[BufferSize];
                                int bytesRead;

                                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await socket.SendAsync(buffer, bytesRead, SocketFlags.None);
                                }
                            }

                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending file: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }

    
}