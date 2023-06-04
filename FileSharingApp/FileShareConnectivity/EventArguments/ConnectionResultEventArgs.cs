
using FileShareConnectivity.Models;

namespace FileShareConnectivity.EventArguments;

public class ConnectionResultEventArgs : EventArgs
{
    public bool IsSuccessConnection { get; private set; }
    public object ConnectionInfo { get; private set; }

    public ConnectionResultEventArgs(bool isSuccessConnection, object connectionInfo)
    {
        IsSuccessConnection = isSuccessConnection;
        ConnectionInfo = connectionInfo;
    }
}
