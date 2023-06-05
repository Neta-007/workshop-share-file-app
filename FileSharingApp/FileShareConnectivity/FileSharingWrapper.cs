using FileShareConnectivity.Interfaces;

namespace FileShareConnectivity;

/*
 * This class exposes the functionality of the services from the DLL to the external world.
 */
public class FileSharingWrapper
{
    public INetworkService NetworkService { get; }
    public IFileTransferService FileTransferService { get; }

    public FileSharingWrapper(IFileTransferService fileTransferService, INetworkService networkService)
    {
        FileTransferService = fileTransferService;
        NetworkService = networkService;
    }
}
