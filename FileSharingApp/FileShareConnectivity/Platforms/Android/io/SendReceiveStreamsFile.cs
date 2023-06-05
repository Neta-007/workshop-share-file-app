using Java.Net;
using Android.Content;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using IOException = Java.IO.IOException;
using File = Java.IO.File;
using AndroidUri = global::Android.Net.Uri;

namespace FileShareConnectivity.Platforms.Android.IO;

internal class SendReceiveStreamsFile
{
    private ILogger<SendReceiveStreamsFile> _logger = MauiApplication.Current.Services.GetService<ILogger<SendReceiveStreamsFile>>();
    private Context _context = global::Android.App.Application.Context;
    private Socket _socket;

    public SendReceiveStreamsFile(Socket socket)
    {
        _socket = socket;
        _logger.LogInformation($"Create SendReceiveStreams to send/receive data. Use the socket input and output streams to communicate between client and server");
    }

    public void SendFile(string filePath)
    {
        _logger.LogDebug($"Start sending file {filePath}");

        Stream inputStream = null;
        Stream outputStream = null;

        try
        {
            AndroidUri fileUri = getValidUriPath(filePath);
            ContentResolver cr = _context.ContentResolver;

            outputStream = _socket.OutputStream;
            inputStream = cr.OpenInputStream(fileUri);
            copyFileStream(inputStream, outputStream);
        }
        catch (IOException e)
        {
            _logger.LogError($"Error sending file: {e.Message}");
        }
        finally
        {
            inputStream?.Dispose();
            outputStream?.Dispose();
        }
    }

    public void ReceiveFile()
    {
        _logger.LogDebug($"Start receiving file");

        string fileReceivePath = null;
        Stream inputStream = null;
        Stream outputStream = null;

        try
        {
            File file = handleReceivedFilePathOperation();

            inputStream = _socket.InputStream;
            outputStream = new FileStream(file.Path, FileMode.OpenOrCreate);
            copyFileStream(inputStream, outputStream);
            fileReceivePath = file.AbsolutePath;
        }
        catch (IOException e)
        {
            _logger.LogError($"Error receiving file: {e.Message}");
        }
        finally 
        {
            outputStream?.Close();
            inputStream?.Close();
        }

        if (fileReceivePath != null)
        {
            _logger.LogDebug($"ReceiveFile finished => new file absolute path: {fileReceivePath}");
            openReceivedFileInDifferentApp(fileReceivePath);
        }
    }

    private void openReceivedFileInDifferentApp(string newFileAbsolutePath)
    {
        /* TODO: not working
        var intent = new Intent();
        intent.SetAction(Intent.ActionView);
        intent.SetDataAndType(global::Android.Net.Uri.Parse("file://" + newFileAbsolutePath), "image/*");
        _context.StartActivity(intent);
        */
    }

    private void copyFileStream(Stream inputStream, Stream outputStream)
    {
        _logger.LogDebug($"Start copying file (input <--> output stream)");
        var buf = new byte[SocketConfiguration.BufferSize];

        try
        {
            int n;

            while ((n = inputStream.Read(buf, 0, buf.Length)) != 0)
            {
                outputStream.Write(buf, 0, n);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error copying file: {e.Message}");
        }
    }

    private File handleReceivedFilePathOperation()
    {
        string fileDirPath = getReceivedFileDirectoriesPath();
        string fileName = generateReceivedFileName();
        string fileFullPath = Path.Combine(fileDirPath, fileName);
        File file = new File(fileFullPath);

        _logger.LogDebug($"The received file going to be saved under: {fileFullPath}");
        if (!file.Exists())
        {
            file.CreateNewFile();
        }

        return file;
    }

    private string getReceivedFileDirectoriesPath()
    {
        // TODO: change the process to save the file under the cache dir (instead of externalStorageDir)
        //       because it meant to be a temp file and saving the file should be under the appropriate location according to his MINE type
        var externalStorageDir = _context.GetExternalFilesDir(null).AbsolutePath;
        string packageName = AppInfo.Current.PackageName;
        File fileDir = new File(externalStorageDir, packageName);

        _logger.LogDebug($"Going to save incoming file under directories path: {fileDir}");
        if (!fileDir.Exists())
        {
            _logger.LogDebug($"The path: {fileDir} doesn't exist in the phone storage. Going to create the desired directories");
            fileDir.Mkdirs();
        }

        return fileDir.Path;
    }

    private string generateReceivedFileName()
    {
        long timestamp = DateTime.Now.Ticks;
        string fileName = $"wifip2pshared-{timestamp}";   // TODO: add later the file type ? => $"wifip2pshared-{timestamp}.jpg" ...

        return fileName;
    }

    private AndroidUri getValidUriPath(string filePath)
    {
        AndroidUri fileUri = null;

        if (isValidUri(filePath))
        {
            try
            {
                fileUri = AndroidUri.Parse(filePath);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error when trying to get valid URI path: URI parsing failed. {e.Message}");
            }
        }
        else
        {
            _logger.LogInformation($"The filePath is not a valid URI, attempt to parse it as a file path");
            File file = new File(filePath);

            if (file.Exists())
            {
                try
                {
                    fileUri = AndroidUri.FromFile(file);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error when trying to get valid URI path: File path parsing failed. {e.Message}");
                }
            }
            else
            {
                _logger.LogError($"Error when trying to get a valid URI path from filePath- {filePath}. The file does not exist");
            }
        }

        _logger.LogDebug($"AndroidUri, fileUri: {fileUri} from filePath: {filePath}");

        return fileUri;
    }

    private bool isValidUri(string uriString)
    {
        const string uriPattern = @"^(?i)(?:[a-z][a-z\d+.-]*:)(?:\\/\\/)(?:[^:@]+(?::[^:@]+)?@)?(?:(?:[a-z\d\u00A1-\uFFFF.-]+(?:\.[a-z\d\u00A1-\uFFFF.-]+)*)|\[(?:(?:IPv6:(?:[\da-fA-F]{1,4}(?::[\da-fA-F]{1,4}){7})|IPv6:(?:(?:[\da-fA-F]{1,4}(?::[\da-fA-F]{1,4}){5}):(?:[\da-fA-F]{1,4}(?::[\da-fA-F]{1,4}){1,2}))|(?:(?:[a-z\d\u00A1-\uFFFF.-]+\.)*[a-z\d\u00A1-\uFFFF.-]+)?))\])(?::\d{2,5})?(?:\/[^\s]*)?$";

        return Regex.IsMatch(uriString, uriPattern);
    }
}