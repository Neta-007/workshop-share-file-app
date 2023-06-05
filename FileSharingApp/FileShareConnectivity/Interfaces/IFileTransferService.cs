
namespace FileShareConnectivity.Interfaces;

public interface IFileTransferService
{
    void StartFileTransfer(object connectionInfo, string filePath);
}
