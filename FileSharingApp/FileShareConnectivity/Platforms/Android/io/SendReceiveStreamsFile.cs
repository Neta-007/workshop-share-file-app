using Java.IO;
using Java.Net;
using IOException = Java.IO.IOException;
using File = Java.IO.File;
using Android.Content;
using Android.Webkit;
using Android.Content.PM;

namespace FileShareConnectivity.Platforms.Android.IO;

internal class SendReceiveStreamsFile
{
    private Socket _socket;     // Use the socket input and output streams to communicate between client and server

    public SendReceiveStreamsFile(Socket socket)
    {
        _socket = socket;
    }

    /*public void SendFile(string filePath)
    {
        try
        {
            using (Stream outputStream = _socket.OutputStream)
            using (Stream inputStream = _socket.InputStream)
            {
                File file = new File(filePath);
                string fileName = file.Name;
                long fileSize = file.Length();

                writeStringToStream(fileName, outputStream);
                writeLongToStream(fileSize, outputStream);

                // Read the acknowledgment message from the receiver
                byte[] acknowledgmentBuffer = new byte[SocketConfiguration.BufferSize];
                int acknowledgmentBytesRead = inputStream.Read(acknowledgmentBuffer);

                if (acknowledgmentBytesRead > 0)
                {
                    string acknowledgmentMessage = System.Text.Encoding.ASCII.GetString(acknowledgmentBuffer, 0, acknowledgmentBytesRead);

                    if (acknowledgmentMessage == SocketConfiguration.AcknowledgmentMessage)
                    {
                        // Start sending the file
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[SocketConfiguration.BufferSize];
                            int bytesRead;

                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                outputStream.Write(buffer, 0, bytesRead);
                            }
                        }

                        outputStream.Flush();
                    }
                }
            }
        }
        catch (IOException e)
        {
            // "Error sending file: " + e.Message
        }
    }*/

    public void SendFile(string filePath)
    {
        var context = global::Android.App.Application.Context;

        try
        {
            var stream = _socket.OutputStream;
            var cr = context.ContentResolver;
            Stream inputStream = null;

            try
            {
                inputStream = cr.OpenInputStream(global::Android.Net.Uri.Parse(filePath));
            }
            catch (Java.IO.FileNotFoundException e)
            {

            }

            CopyFile(inputStream, stream);
        }
        catch (IOException e)
        {
        }
    }

    public void ReceiveFile(Context context)
    {
        Task.Factory.StartNew(() =>
        {
            try
            {
                var f = new File(global::Android.OS.Environment.ExternalStorageDirectory + "/"
                                 + AppInfo.Current.PackageName + "/wifip2pshared-" + DateTime.Now.Ticks + ".jpg");
                var dirs = new File(f.Parent);
                if (!dirs.Exists())
                    dirs.Mkdirs();
                f.CreateNewFile();

                var inputStream = _socket.InputStream;
                CopyFile(inputStream, new FileStream(f.ToString(), FileMode.OpenOrCreate));

                return f.AbsolutePath;
            }
            catch (IOException e)
            {
                return null;
            }
        })
        .ContinueWith(result =>
        {
            if (result != null)
            {
                var intent = new Intent();
                intent.SetAction(Intent.ActionView);
                intent.SetDataAndType(global::Android.Net.Uri.Parse("file://" + result.Result), "image/*");
                context.StartActivity(intent);
            }
        });

    }

    public static bool CopyFile(Stream inputStream, Stream outputStream)
    {
        var buf = new byte[1024];
        try
        {
            int n;
            while ((n = inputStream.Read(buf, 0, buf.Length)) != 0)
                outputStream.Write(buf, 0, n);
            outputStream.Close();
            inputStream.Close();
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public void ReceiveFileToSaveInDifferentApp(Context context)
    {
        using (Stream outputStream = _socket.OutputStream)
        using (Stream inputStream = _socket.InputStream)
        {
            string fileName = readStringFromStream(inputStream);
            long fileSize = readLongFromStream(inputStream);

            // Create a temporary file to save the received data
            string tempFilePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[SocketConfiguration.BufferSize];
                int bytesRead;
                long totalBytesReceived = 0;

                // Receive and write file data
                while ((bytesRead = inputStream.Read(buffer)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    totalBytesReceived += bytesRead;

                    // Check if all bytes have been received
                    if (totalBytesReceived >= fileSize)
                    {
                        break;
                    }
                }
            }

            // Create a content URI for the temporary file
            global::Android.Net.Uri fileUri = FileProvider.GetUriForFile(context, context.PackageName + ".fileprovider", new File(tempFilePath));

            // Get the responsible app to handle the file
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(fileUri, "application/*");
            intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.NoHistory);
            intent.AddFlags(ActivityFlags.ClearTop);
            IList<ResolveInfo> resolvedActivities = context.PackageManager.QueryIntentActivities(intent, 0);

            if (resolvedActivities.Count > 0)
            {
                // Let the receiver choose the responsible app to handle the file
                intent.SetFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            else
            {
                // No responsible app found, handle the scenario accordingly
            }

            // Send acknowledgment message to the sender
            byte[] acknowledgmentBuffer = System.Text.Encoding.ASCII.GetBytes(SocketConfiguration.AcknowledgmentMessage);
            outputStream.Write(acknowledgmentBuffer, 0, acknowledgmentBuffer.Length);
            outputStream.Flush();
        }
    }

    /*public void ReceiveFile(string saveFolderPath)
    {
        try
        {
            using (Stream outputStream = _socket.OutputStream)
            using (Stream inputStream = _socket.InputStream)
            {
                // Read file name from the input stream
                string fileName = readStringFromStream(inputStream);

                // Read file size from the input stream
                long fileSize = readLongFromStream(inputStream);

                // Create a new file to save the received data
                string saveFilePath = Path.Combine(saveFolderPath, fileName);
                using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[SocketConfiguration.BufferSize];
                    int bytesRead;
                    long totalBytesReceived = 0;

                    // Receive and write file data
                    while ((bytesRead = inputStream.Read(buffer)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        totalBytesReceived += bytesRead;

                        // Check if all bytes have been received
                        if (totalBytesReceived >= fileSize)
                        {
                            break;
                        }
                    }
                }

                // Send acknowledgment message to the sender
                byte[] acknowledgmentBuffer = System.Text.Encoding.ASCII.GetBytes(SocketConfiguration.AcknowledgmentMessage);
                //outputStream.Write(acknowledgmentBuffer);
                outputStream.Write(acknowledgmentBuffer, 0, acknowledgmentBuffer.Length);
                outputStream.Flush();
            }
        }
        catch (IOException e)
        {
            // "Error receiving file: " + e.Message
        }
    }*/

    private string readStringFromStream(Stream inputStream)
    {
        using (StreamReader reader = new StreamReader(inputStream))
        {
            return reader.ReadLine();
        }
    }

    private long readLongFromStream(Stream inputStream)
    {
        byte[] longBytes = new byte[8];
        inputStream.Read(longBytes, 0, longBytes.Length);
        return BitConverter.ToInt64(longBytes, 0);
    }

    private void writeStringToStream(string value, Stream outputStream)
    {
        using (StreamWriter writer = new StreamWriter(outputStream))
        {
            writer.WriteLine(value);
        }
    }

    private void writeLongToStream(long value, Stream outputStream)
    {
        byte[] longBytes = BitConverter.GetBytes(value);
        outputStream.Write(longBytes, 0, longBytes.Length);
        outputStream.Flush();
    }
}