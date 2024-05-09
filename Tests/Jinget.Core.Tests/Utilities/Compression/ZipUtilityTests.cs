using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Jinget.Core.Utilities.Compression.Tests;

[TestClass()]
public class ZipUtilityTests
{
    ZipUtility zip;
    [TestInitialize]
    public void Initialize() => zip = new ZipUtility();

    [TestMethod()]
    public async Task should_compress_and_chunkAsync()
    {
        FileInfo file = new("sample.txt");
        int maxDOP = 5;
        int eachFileMaxSize = 4;

        List<FileInfo> files = [file];
        await zip.CompressAsync([.. files], files[0].DirectoryName, maxDOP, eachFileMaxSize);

        Assert.IsTrue(file.Directory.GetFiles("sample.txt.z??").Length != 0);
    }

    [TestMethod()]
    public async Task should_compress_using_password_and_chunkAsync()
    {
        FileInfo file = new("sample.txt");
        string password = "123";
        List<FileInfo> files = [file];
        await zip.CompressAsync([.. files], files[0].DirectoryName, password: password);

        Assert.IsTrue(file.Directory.GetFiles("sample.txt.z??").Length != 0);
    }

    [TestMethod()]
    public async Task should_decompressed_fileAsync()
    {
        string fileName = Guid.NewGuid() + ".txt";
        using (var tw = new StreamWriter(fileName, true))
        {
            await tw.WriteLineAsync("sample text");
        }
        string password = "123";
        var tobeCompressed = new FileInfo[] { new(fileName) };
        await zip.CompressAsync(tobeCompressed, tobeCompressed[0].DirectoryName, password: password);
        File.Delete(fileName);

        FileInfo file = new($"{fileName}.zip");
        int maxDOP = 5;

        List<FileInfo> files = [file];
        await zip.DecompressAsync([.. files], files[0].DirectoryName, maxDOP);

        Assert.IsTrue(file.Directory.GetFiles(fileName).Length != 0);
    }

    [TestMethod()]
    public async Task should_decompressed_using_passwordAsync()
    {
        string fileName = Guid.NewGuid() + ".txt";
        using (var tw = new StreamWriter(fileName, true))
        {
            await tw.WriteLineAsync("sample text");
        }
        string password = "123";
        var tobeCompressed = new FileInfo[] { new(fileName) };
        await zip.CompressAsync(tobeCompressed, tobeCompressed[0].DirectoryName, password: password);
        File.Delete(fileName);

        FileInfo file = new($"{fileName}.zip");
        List<FileInfo> files = [file];
        await zip.DecompressAsync([.. files], files[0].DirectoryName, password: password);

        Assert.IsTrue(file.Directory.GetFiles(fileName).Length != 0);
    }
}