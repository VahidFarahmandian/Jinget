using System.IO.Compression;
using System.Threading;

namespace Jinget.Core.Utilities.Compression;

public class ZipUtility
{
    /// <param name="files">each file will be compressed idependently</param>
    /// <param name="maxDOP">max degree of parallelism</param>
    /// <param name="eachFileMaxSize">each file will be compressed and then chunked based on this parameter. eachFileMaxSize is in MB</param>
    /// <param name="path">location to store compressed files</param>
    /// <param name="password">compressed files can also become password protected</param>
    /// <param name="compressionLevel">It is up to you to choose between best speed to high compression</param>
    public static async Task CompressAsync(
        FileInfo[] files,
        string path,
        int maxDOP = 1,
        int eachFileMaxSize = 5,
        string password = "",
        CompressionLevel compressionLevel = CompressionLevel.Fastest
        )
    {
        ArgumentNullException.ThrowIfNull(files);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
        }

        if (eachFileMaxSize > 2048)
            throw new ArgumentException($"{nameof(eachFileMaxSize)} exceeds the max allowed value. max allowed value is 2048, which is equal to 2GB");
        if (eachFileMaxSize < 1)
            eachFileMaxSize = 5;

        var allTasks = new List<Task>();

        var threadCount = files.Length < maxDOP ? files.Length : maxDOP;
        var throttler = new SemaphoreSlim(threadCount);
        CancellationToken token = new();
        foreach (var file in files)
        {
            await throttler.WaitAsync(token);
            allTasks.Add(Task.Run(async () =>
            {
                if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "jinget", "zip")))
                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "jinget", "zip"));

                var fileName = Path.GetFileNameWithoutExtension(file.Name);
                string compressedFile = Path.Combine(Path.GetTempPath(), "jinget", "zip", $"{fileName}.zip");
                string encryptedFile = Path.Combine(Path.GetTempPath(), "jinget", "zip", $"{fileName}.enc");
                string finalResultFile = "";
                int chunkSize = eachFileMaxSize * 1024 * 1024;
                try
                {
                    // Compress the file into a ZIP archive
                    await CompressFileAsync(file.FullName, compressedFile, compressionLevel);
                    finalResultFile = compressedFile;
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        // Encrypt the ZIP file with AES
                        await EncryptFileAsync(compressedFile, encryptedFile, password);
                        finalResultFile = encryptedFile;
                    }

                    // Split the encrypted file into chunks
                    await SplitZipFileAsync(finalResultFile, chunkSize, file.Directory?.FullName);
                }
                finally
                {
                    Directory.Delete(Path.Combine(Path.GetTempPath(), "jinget", "zip"), true);
                    throttler.Release();
                }
            }, token));
        }

        await Task.WhenAll(allTasks);
    }

    static async Task CompressFileAsync(string sourceFile, string zipFile, CompressionLevel compressionLevel)
    {
        using FileStream zipStream = new(zipFile, FileMode.Create, FileAccess.Write);
        using ZipArchive archive = new(zipStream, ZipArchiveMode.Create);
        await Task.Run(() => archive.CreateEntryFromFile(sourceFile, Path.GetFileName(sourceFile), compressionLevel));
    }
    static async Task EncryptFileAsync(string inputFile, string outputFile, string password)
    {
        byte[] saltBytes = new byte[16];
        RandomNumberGenerator.Fill(saltBytes);

        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        // Derive a key using Rfc2898
        Rfc2898DeriveBytes rfc2898 = new(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        byte[] keyBytes = rfc2898.GetBytes(32);
        byte[] ivBytes = rfc2898.GetBytes(16);

        using FileStream fsCrypt = new(outputFile, FileMode.Create, FileAccess.Write);
        await fsCrypt.WriteAsync(saltBytes);
        await fsCrypt.WriteAsync(ivBytes);

        using Aes aesAlg = Aes.Create();
        aesAlg.Key = keyBytes;
        aesAlg.IV = ivBytes;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using CryptoStream csEncrypt = new(fsCrypt, encryptor, CryptoStreamMode.Write);
        using (FileStream fsIn = new(inputFile, FileMode.Open, FileAccess.Read))
        {
            await fsIn.CopyToAsync(csEncrypt);
        }
        csEncrypt.FlushFinalBlock();
    }
    static async Task SplitZipFileAsync(string zipFile, int chunkSize, string? destinationDirectory)
    {
        if (destinationDirectory == null) destinationDirectory = "";
        FileInfo fileInfo = new(zipFile);
        string fileName = Path.GetFileNameWithoutExtension(zipFile);

        if (fileInfo.Length <= chunkSize)
        {
            string destinationFileName = Path.Combine(destinationDirectory, $"{fileName}.zip");
            File.Move(zipFile, destinationFileName);
            return;
        }

        using FileStream fs = new(zipFile, FileMode.Open, FileAccess.Read);
        byte[] buffer = new byte[chunkSize];
        int bytesRead;
        int partNumber = 1;

        while ((bytesRead = await fs.ReadAsync(buffer)) > 0)
        {
            //first file's name will not be changed
            string partFile = partNumber == 1 ? $"{fileName}.zip" : $"{fileName}-part{partNumber}.zip";
            using (FileStream partFs = new(partFile, FileMode.Create, FileAccess.Write))
            {
                await partFs.WriteAsync(buffer.AsMemory(0, bytesRead));
            }
            partNumber++;
        }
    }

    /// <param name="files">list of files to extract</param>
    /// <param name="maxDOP">max degree of parallelism</param>
    /// <param name="path">location to extract the files</param>
    /// <param name="password">password used to extract the compressed files</param>
    public static async Task DecompressAsync(
        FileInfo[] files,
        string path,
        int maxDOP = 1,
        string password = "")
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
        }

        ArgumentNullException.ThrowIfNull(files);

        var allTasks = new List<Task>();
        var threadCount = files.Length < maxDOP ? files.Length : maxDOP;
        var throttler = new SemaphoreSlim(threadCount);
        var token = new CancellationToken();
        foreach (var file in files)
        {
            await throttler.WaitAsync(token);

            allTasks.Add(Task.Run(async () =>
            {
                if (!Directory.Exists(Path.Combine(Path.GetTempPath(), "jinget", "zip")))
                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "jinget", "zip"));
                try
                {
                    string fileName = file.FullName;
                    string reassembledFile = Path.Combine(Path.GetTempPath(), "jinget", "zip", Path.GetFileName(Path.ChangeExtension(Path.GetTempFileName(), ".zip")));
                    string decryptedFile = Path.Combine(Path.GetTempPath(), "jinget", "zip", Path.GetFileName(Path.ChangeExtension(Path.GetTempFileName(), ".zip")));

                    // Reassemble the split files
                    await ReassembleSplitFileAsync(fileName, reassembledFile);

                    string finalResultFile = reassembledFile;
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        // Decrypt the file
                        await DecryptFileAsync(reassembledFile, decryptedFile, password);
                        finalResultFile = decryptedFile;
                    }
                    // Decompress the decrypted file
                    await DecompressFileAsync(finalResultFile, path);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    Directory.Delete(Path.Combine(Path.GetTempPath(), "jinget", "zip"), true);
                    throttler.Release();
                }
            }, token));
        }
        await Task.WhenAll(allTasks);
    }

    static async Task ReassembleSplitFileAsync(string encryptedFile, string outputFile)
    {
        // Assuming split files are named part1.enc, part2.enc, etc.
        string fileName = Path.GetFileNameWithoutExtension(encryptedFile);
        int partNumber = 1;
        string partFile = $"{fileName}.zip";

        while (File.Exists(partFile))
        {
            using (FileStream fsIn = File.OpenRead(partFile))
            {
                using FileStream fsOut = File.Open(outputFile, FileMode.Append, FileAccess.Write);
                await fsIn.CopyToAsync(fsOut);
            }
            partNumber++;
            partFile = $"{fileName}-part{partNumber}.zip";
        }
    }
    static async Task DecryptFileAsync(string encryptedFile, string outputFile, string password)
    {
        using FileStream fsCrypt = new(encryptedFile, FileMode.Open);
        try
        {
            // Read salt and IV
            byte[] saltBytes = new byte[16];
            await fsCrypt.ReadAsync(saltBytes.AsMemory(0, 16));

            byte[] ivBytes = new byte[16];
            await fsCrypt.ReadAsync(ivBytes.AsMemory(0, 16));

            // Derive key
            Rfc2898DeriveBytes rfc2898 = new(password, saltBytes, 10000, HashAlgorithmName.SHA256);
            byte[] keyBytes = rfc2898.GetBytes(32);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using CryptoStream csDecrypt = new(fsCrypt, decryptor, CryptoStreamMode.Read);
            using FileStream fsOut = new(outputFile, FileMode.Create, FileAccess.Write);
            await csDecrypt.CopyToAsync(fsOut);
        }
        catch (CryptographicException ex)
        {
            throw new Exception("Decryption failed: " + ex.Message);
        }
    }
    static async Task DecompressFileAsync(string compressedFile, string path)
    {
        using FileStream zipStream = new(compressedFile, FileMode.Open);
        using ZipArchive archive = new(zipStream);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string filePath = Path.Combine(path, entry.FullName);
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath != null)
                Directory.CreateDirectory(directoryPath);

            using FileStream fileStream = File.Create(filePath);
            using Stream entryStream = entry.Open();
            await entryStream.CopyToAsync(fileStream);
        }
    }
}
