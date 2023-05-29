namespace FileShareConnectivity.Platforms.Android.IO;

internal static class SocketConfiguration
{
    public static readonly byte[] MessageDelimiterBytes = System.Text.Encoding.ASCII.GetBytes("<EOF>");
    public const int BufferSize = 1024;
    public const string AcknowledgmentMessage = "ACK";

    public static int SocketPort { get; private set; } = 8888;

    public static int SocketTimeout { get; } = 500;

    public static int DrillNewPort()
    {
        //TODO: implement a drill new port mechanism

        return SocketPort;
    }
}