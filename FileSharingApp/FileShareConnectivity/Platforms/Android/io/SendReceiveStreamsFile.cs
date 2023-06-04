using Java.Net;
using IOException = Java.IO.IOException;
using File = Java.IO.File;
using AndroidUri = global::Android.Net.Uri;
using Android.Content;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Android.Provider;
using Android.Webkit;
using Android.Widget;

namespace FileShareConnectivity.Platforms.Android.IO;

internal class SendReceiveStreamsFile
{
    private ILogger<SendReceiveStreamsFile> _logger = MauiApplication.Current.Services.GetService<ILogger<SendReceiveStreamsFile>>();
    Context _context = global::Android.App.Application.Context;
    private Socket _socket;     // Use the socket input and output streams to communicate between client and server

    public SendReceiveStreamsFile(Socket socket)
    {
        _socket = socket;
    }

    public void SendFile(string filePath)
    {
        _logger.LogDebug($"SendReceiveStreamsFile SendFile {filePath}");

        var file = new Java.IO.File(filePath);

        if (file.Exists())
        {
            try
            {
                var stream = _socket.OutputStream;
                var cr = _context.ContentResolver;
                Stream inputStream = null;

                try
                {
                    AndroidUri fileUri = getValidUriPath(filePath);
                    _logger.LogDebug($"SendFile AndroidUri fileUri: {fileUri}");
                    inputStream = cr.OpenInputStream(fileUri);
                    CopyFile(inputStream, stream);
                }
                catch (Java.IO.FileNotFoundException e)
                {
                    _logger.LogError($"Error sending file, FileNotFoundException: {e.Message}");
                }
            }
            catch (IOException e)
            {
                _logger.LogError($"Error sending file: {e.Message}");
            }
        }
        else
        {
            _logger.LogError($"Error sending file: File not exist");
        }
    }

    public void ReceiveFile()
    {
        string fileReceivePath = null;

        try
        {
            _logger.LogDebug($"SendReceiveStreamsFile ReceiveFile");
            //File mediaStorageDir = _context.GetExternalFilesDir(null);
            var externalStorageDir = _context.GetExternalFilesDir(null).AbsolutePath;
            //var externalStorageDir = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures);

            //var externalStorageDir = global::Android.OS.Environment.ExternalStorageDirectory;
            string packageName = AppInfo.Current.PackageName;
            _logger.LogDebug($"SendReceiveStreamsFile mediaStorageDir {externalStorageDir}");
            _logger.LogDebug($"SendReceiveStreamsFile ReceiveFile {packageName}");

            File fileDir = new File(externalStorageDir, packageName);
            _logger.LogDebug($"SendReceiveStreamsFile fileDir {fileDir}");

            _logger.LogDebug($"SendReceiveStreamsFile !fileDir.Exists(): {!fileDir.Exists()}");
            if (!fileDir.Exists())
            {
                fileDir.Mkdirs();
            }
            _logger.LogDebug($"SendReceiveStreamsFile after !fileDir.Exists(): {!fileDir.Exists()}");

            long timestamp = DateTime.Now.Ticks;
            string filePath = System.IO.Path.Combine(fileDir.Path, $"wifip2pshared-{timestamp}");   // TODO: add later the file type
            _logger.LogDebug($"SendReceiveStreamsFile filePath {filePath}");

            File file = new File(filePath);
            _logger.LogDebug($"SendReceiveStreamsFile !file.Exists(): {!file.Exists()}");
            if (!file.Exists())
            {
                file.CreateNewFile();
            }
            _logger.LogDebug($"SendReceiveStreamsFile after !file.Exists(): {!file.Exists()}");
            _logger.LogDebug($"SendReceiveStreamsFile file {file}");

            Stream inputStream = _socket.InputStream;
            Stream outputStream = new FileStream(file.Path, FileMode.OpenOrCreate);

            CopyFile(inputStream, outputStream);

            fileReceivePath = file.AbsolutePath;
        }
        catch (IOException e)
        {
            _logger.LogError($"Error receiving file: {e.Message}");
        }

        _logger.LogDebug($"SendReceiveStreamsFile ReceiveFile => fileReceivePath: {fileReceivePath}");

        /* TODO: not working
        if (fileReceivePath != null)
        {
            var intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(global::Android.Net.Uri.Parse("file://" + fileReceivePath), "image/*");
            _context.StartActivity(intent);
        }*/
    }

    public bool CopyFile(Stream inputStream, Stream outputStream)
    {
        _logger.LogDebug($"SendReceiveStreamsFile CopyFile");
        var buf = new byte[1024];
        bool res = true;

        try
        {
            int n;

            while ((n = inputStream.Read(buf, 0, buf.Length)) != 0)
            {
                outputStream.Write(buf, 0, n);
            }

            outputStream.Close();
            inputStream.Close();
        }
        catch (Exception e)
        {
            _logger.LogError($"Error copying file: {e.Message}");
            res = false;
        }

        return res;
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
                _logger.LogError($"Error getValidUriPath: URI parsing failed. {e.Message}");
            }
        }
        else
        {
            _logger.LogInformation($"getValidUriPath: The filePath is not a valid URI, attempt to parse it as a file path");
            Java.IO.File file = new Java.IO.File(filePath);

            if (file.Exists())
            {
                try
                {
                    fileUri = AndroidUri.FromFile(new Java.IO.File(filePath));
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error getValidUriPath: File path parsing failed. {e.Message}");
                }
            }
            else
            {
                _logger.LogError($"Error getValidUriPath: The file does not exist");
            }
        }

        return fileUri;
    }

    private bool isValidUri(string uriString)
    {
        const string uriPattern = @"^(?i)(?:[a-z][a-z\d+.-]*:)(?:\\/\\/)(?:[^:@]+(?::[^:@]+)?@)?(?:(?:[a-z\d\u00A1-\uFFFF.-]+(?:\.[a-z\d\u00A1-\uFFFF.-]+)*)|\[(?:(?:IPv6:(?:[\da-fA-F]{1,4}(?::[\da-fA-F]{1,4}){7})|IPv6:(?:(?:[\da-fA-F]{1,4}(?::[\da-fA-F]{1,4}){5}):(?:[\da-fA-F]{1,4}(?::[\da-fA-F]{1,4}){1,2}))|(?:(?:[a-z\d\u00A1-\uFFFF.-]+\.)*[a-z\d\u00A1-\uFFFF.-]+)?))\])(?::\d{2,5})?(?:\/[^\s]*)?$";

        return Regex.IsMatch(uriString, uriPattern);
    }
}