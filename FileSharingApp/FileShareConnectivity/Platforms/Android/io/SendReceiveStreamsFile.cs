using Java.IO;
using Java.Net;
using IOException = Java.IO.IOException;
using File = Java.IO.File;

namespace FileShareConnectivity.Platforms.Android.io;

internal class SendReceiveStreamsFile
{
    private Socket _socket;

    public SendReceiveStreamsFile(Socket socket)
    {
        _socket = socket;
    }

    public void SendFile(string filePath)
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
                //writeFileNameAndSizeToOutputStream(file);

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
    }

    public void ReceiveFile(string saveFolderPath)
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
    }

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