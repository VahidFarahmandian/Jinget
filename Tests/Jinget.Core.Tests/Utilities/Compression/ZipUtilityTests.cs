using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Jinget.Core.Utilities.Compression.Tests
{
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

            List<FileInfo> files = new()
            {
                file
            };
            await zip.CompressAsync(files.ToArray(), files[0].DirectoryName, maxDOP, eachFileMaxSize);

            Assert.IsTrue(file.Directory.GetFiles("sample.txt.z??").Any());
        }

        [TestMethod()]
        public async Task should_compress_using_password_and_chunkAsync()
        {
            FileInfo file = new("sample.txt");
            string password = "123";
            List<FileInfo> files = new()
            {
                file
            };
            await zip.CompressAsync(files.ToArray(), files[0].DirectoryName, password: password);

            Assert.IsTrue(file.Directory.GetFiles("sample.txt.z??").Any());
        }

        [TestMethod()]
        public async Task should_decompressed_file()
        {
            string fileName = Guid.NewGuid() + ".txt";
            using (var tw = new StreamWriter(fileName, true))
            {
                tw.WriteLine("sample text");
            }
            string password = "123";
            var tobeCompressed = new FileInfo[] { new FileInfo(fileName) };
            await zip.CompressAsync(tobeCompressed, tobeCompressed[0].DirectoryName, password: password);
            File.Delete(fileName);

            FileInfo file = new($"{fileName}.zip");
            int maxDOP = 5;

            List<FileInfo> files = new()
            {
                file
            };
            await zip.DecompressAsync(files.ToArray(), files[0].DirectoryName, maxDOP);

            Assert.IsTrue(file.Directory.GetFiles(fileName).Any());
        }

        [TestMethod()]
        public async Task should_decompressed_using_password()
        {
            string fileName = Guid.NewGuid() + ".txt";
            using (var tw = new StreamWriter(fileName, true))
            {
                tw.WriteLine("sample text");
            }
            string password = "123";
            var tobeCompressed = new FileInfo[] { new FileInfo(fileName) };
            await zip.CompressAsync(tobeCompressed, tobeCompressed[0].DirectoryName, password: password);
            File.Delete(fileName);

            FileInfo file = new($"{fileName}.zip");
            List<FileInfo> files = new()
            {
                file
            };
            await zip.DecompressAsync(files.ToArray(), files[0].DirectoryName, password: password);

            Assert.IsTrue(file.Directory.GetFiles(fileName).Any());
        }
    }
}