
namespace FileShareConnectivity.Platforms.Android.IO;

internal abstract class BaseFileSocket : IDisposable
{
    protected SendReceiveStreamsFile _sendReceiveStreams;

    public abstract void SendFile(string filePath);
    public abstract void ReceiveFileToSaveInDifferentApp();
    public abstract void Close();
    public abstract void Dispose();
}
