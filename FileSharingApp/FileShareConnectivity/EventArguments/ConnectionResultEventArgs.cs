
using FileShareConnectivity.Models;

namespace FileShareConnectivity.EventArguments;

public class ConnectionResultEventArgs : EventArgs
{
    public bool IsSuccessConnection { get; private set; }
    public object ConnectionInfo { get; private set; }
    //public NearbyDevice RecieverDevice { get; internal set; }

    public ConnectionResultEventArgs(bool isSuccessConnection, object connectionInfo)//, NearbyDevice recieverDevice = null)
    {
        IsSuccessConnection = isSuccessConnection;
        ConnectionInfo = connectionInfo;
        //RecieverDevice = recieverDevice;
    }
}
