using System.IO;
using System.Net.Sockets;
using Android.Content.Res;
using Android.Net.Wifi.P2p;
using Android.Views;
using Android.Widget;
using FileShareConnectivity.Interfaces;
using View = Microsoft.Maui.Controls.View;

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
                    
                }
            }
        }
    }

    
}