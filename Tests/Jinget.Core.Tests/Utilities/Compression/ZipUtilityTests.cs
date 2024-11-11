using System.IO;
using System.Security.Cryptography;
using Jinget.Core.Utilities.Compression;

namespace Jinget.Core.Tests.Utilities.Compression;

[TestClass]
public class ZipUtilityTests
{
    string Create15MBFile()
    {
        string filename = Path.ChangeExtension(Path.GetTempFileName(), ".txt");
        long fileSizeInBytes = 15 * 1024 * 1024; // 15MB
        using (FileStream fs = new(filename, FileMode.Create, FileAccess.Write))
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] buffer = new byte[1024 * 1024]; // 1 MB buffer
            long bytesWritten = 0;

            while (bytesWritten < fileSizeInBytes)
            {
                int bytesToWrite = (int)Math.Min(buffer.Length, fileSizeInBytes - bytesWritten);
                rng.GetBytes(buffer, 0, bytesToWrite);
                fs.Write(buffer, 0, bytesToWrite);
                bytesWritten += bytesToWrite;
            }
        }
        return filename;
    }
    private async Task<FileInfo> CompressAsync(string fileName, string password = "")
    {
        using (var tw = new StreamWriter(fileName, true))
        {
            await tw.WriteAsync("sample text");
        }
        var tobeCompressed = new FileInfo[] { new(fileName) };
        await ZipUtility.CompressAsync(tobeCompressed, tobeCompressed[0].DirectoryName, password: password);
        File.Delete(fileName);
        return tobeCompressed[0];
    }

    private async Task<FileInfo> CompressLargeFileAsync(string password = "", int eachFileMaxSize = 1)
    {
        string fileName = Create15MBFile();
        FileInfo file = new(fileName);
        int maxDOP = 5;

        List<FileInfo> files = [file];
        await ZipUtility.CompressAsync([.. files], files[0].DirectoryName, maxDOP, eachFileMaxSize, password);
        return file;
    }

    [TestCleanup]
    public void Cleanup()
    {
        string[] zipFiles = Directory.GetFiles(".", "*.zip");
        foreach (string zipFile in zipFiles)
        {
            File.Delete(zipFile);
        }
        string[] txtFiles = Directory.GetFiles(".", "*.txt");
        foreach (string txtFile in txtFiles)
        {
            File.Delete(txtFile);
        }
    }

    [TestMethod]
    public async Task should_throw_ArgumentException_when_files_nullAsync()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await ZipUtility.CompressAsync(null, "path"));
    }

    [TestMethod]
    public async Task should_compress_empty_fileAsync()
    {
        var file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
        File.Create(file).Close();

        var compressedFile = await CompressAsync(file);
        string compressedFileName = Path.GetFileNameWithoutExtension(compressedFile.Name);
        Assert.IsTrue(compressedFile.Directory.GetFiles($"{compressedFileName}.zip").Length == 1);
    }

    [TestMethod]
    public async Task should_compressAsync()
    {
        var compressedFile = await CompressAsync($"{Guid.NewGuid()}.txt");
        string compressedFileName = Path.GetFileNameWithoutExtension(compressedFile.Name);
        Assert.IsTrue(compressedFile.Directory.GetFiles($"{compressedFileName}.zip").Length == 1);
    }

    [TestMethod]
    public async Task should_decompressAsync()
    {
        var compressedFile = await CompressAsync($"{Guid.NewGuid()}.txt");
        string compressedFileName = Path.GetFileNameWithoutExtension(compressedFile.Name);

        FileInfo file = new($"{compressedFileName}.zip");
        List<FileInfo> files = [file];
        await ZipUtility.DecompressAsync([.. files], files[0].DirectoryName);

        Assert.IsTrue(file.Directory.GetFiles(compressedFile.Name).Length == 1);
        string content = await File.ReadAllTextAsync($"{compressedFileName}.txt");
        Assert.AreEqual("sample text", content);
    }

    [TestMethod]
    public async Task should_compress_large_file_and_chunkAsync()
    {
        var compressedFile = await CompressLargeFileAsync(eachFileMaxSize: 1);
        string compressedFileName = $"{Path.GetFileNameWithoutExtension(compressedFile.Name)}";

        Assert.IsTrue(Directory.GetFiles(".", $"{compressedFileName}.zip").Length == 1);
        Assert.IsTrue(Directory.GetFiles(".", $"{compressedFileName}-part?.zip").Length > 0);
    }

    [TestMethod]
    public async Task should_decompress_large_file_with_chunkAsync()
    {
        var compressedFile = await CompressLargeFileAsync(eachFileMaxSize: 1);
        string compressedFileName = $"{Path.GetFileNameWithoutExtension(compressedFile.Name)}";

        FileInfo file = new($"{compressedFileName}.zip");
        List<FileInfo> files = [file];
        await ZipUtility.DecompressAsync([.. files], files[0].DirectoryName);

        Assert.IsTrue(Directory.GetFiles(".", $"{compressedFileName}.txt").Length == 1);
    }

    [TestMethod]
    public async Task should_compress_using_passwordAsync()
    {
        var compressedFile = await CompressAsync($"{Guid.NewGuid()}.txt", password: "123");
        string compressedFileName = $"{Path.GetFileNameWithoutExtension(compressedFile.Name)}";

        Assert.IsTrue(compressedFile.Directory.GetFiles($"{compressedFileName}.zip").Length == 1);
    }

    [TestMethod]
    public async Task should_decompress_using_passwordAsync()
    {
        var compressedFile = await CompressAsync($"{Guid.NewGuid()}.txt", password: "123");
        string compressedFileName = $"{Path.GetFileNameWithoutExtension(compressedFile.Name)}";

        FileInfo file = new($"{compressedFileName}.zip");
        List<FileInfo> files = [file];
        await ZipUtility.DecompressAsync([.. files], files[0].DirectoryName, password: "123");

        Assert.IsTrue(file.Directory.GetFiles($"{compressedFileName}.txt").Length == 1);
        string content = await File.ReadAllTextAsync($"{compressedFileName}.txt");
        Assert.AreEqual("sample text", content);
    }

    [TestMethod]
    public async Task should_throw_exception_for_invalid_password_in_decompressing_using_passwordAsync()
    {
        var compressedFile = await CompressAsync($"{Guid.NewGuid()}.txt", password: "123");
        string compressedFileName = $"{Path.GetFileNameWithoutExtension(compressedFile.Name)}";

        FileInfo file = new($"{compressedFileName}.zip");
        List<FileInfo> files = [file];
        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await ZipUtility.DecompressAsync([.. files], files[0].DirectoryName, password: "456");
        });
    }

    [TestMethod]
    public async Task should_compress_large_file_using_password_and_chunkAsync()
    {
        var compressedFile = await CompressLargeFileAsync(password: "123", eachFileMaxSize: 1);
        string compressedFileName = $"{Path.GetFileNameWithoutExtension(compressedFile.Name)}";

        Assert.IsTrue(Directory.GetFiles(".", $"{compressedFileName}.zip").Length == 1);
        Assert.IsTrue(Directory.GetFiles(".", $"{compressedFileName}-part?.zip").Length > 0);
    }

    [TestMethod]
    public async Task should_decompress_large_file_using_password_and_chunkAsync()
    {
        var compressedFile = await CompressLargeFileAsync(password: "123", eachFileMaxSize: 1);
        string compressedFileName = $"{Path.GetFileNameWithoutExtension(compressedFile.Name)}";

        FileInfo file = new($"{compressedFileName}.zip");
        List<FileInfo> files = [file];
        await ZipUtility.DecompressAsync([.. files], files[0].DirectoryName, password: "123");

        Assert.IsTrue(Directory.GetFiles(".", $"{compressedFileName}.txt").Length == 1);
    }
}