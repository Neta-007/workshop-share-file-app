using FileShareConnectivity.Interfaces;

namespace FileShareConnectivity;

/*
 * This class that exposes the functionality of the services from the DLL to the external world.
 */
public class FileSharingWrapper
{
    public INetworkService NetworkService { get; internal set; }
    public IFileTransferService FileTransferService { get; internal set; }

    public FileSharingWrapper()
    {
    }
}
