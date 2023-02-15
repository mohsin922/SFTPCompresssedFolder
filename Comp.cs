using Renci.SshNet;
using System.IO.Compression;

// Establish SFTP connection
var connectionInfo = new ConnectionInfo("hostname", "username", new PasswordAuthenticationMethod("username", "password"));
using (var client = new SftpClient(connectionInfo))
{
    client.Connect();

    // Specify the compressed folder to unzip and download files from
    string compressedFolder = "/path/to/compressed/folder.zip";
    string extractToFolder = "/path/to/extract/folder/";

    // Download the compressed folder
    using (var compressedFileStream = new MemoryStream())
    {
        client.DownloadFile(compressedFolder, compressedFileStream);

        // Unzip the compressed folder
        using (var archive = new ZipArchive(compressedFileStream))
        {
            foreach (var entry in archive.Entries)
            {
                // Extract each file from the compressed folder
                string extractFilePath = Path.Combine(extractToFolder, entry.FullName);

                if (!Directory.Exists(Path.GetDirectoryName(extractFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(extractFilePath));
                }

                using (var entryStream = entry.Open())
                using (var extractStream = File.Create(extractFilePath))
                {
                    entryStream.CopyTo(extractStream);
                }
            }
        }
    }

    client.Disconnect();
}
