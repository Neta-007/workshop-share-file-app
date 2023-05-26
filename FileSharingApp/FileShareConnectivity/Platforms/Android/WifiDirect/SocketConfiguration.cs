namespace FileShareConnectivity.Platforms.Android.WifiDirect;

internal static class SocketConfiguration
{
    public static int SocketPort { get; private set; } = 8888;
    
    public static int SocketTimeout { get; }= 500;

    public static int DrillNewPort()
    {
        //TODO: implement a drill new port mechanism

        return SocketPort;
    }
    
}