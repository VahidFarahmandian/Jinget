using Ionic.Zip;
using Ionic.Zlib;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jinget.Core.Utilities.Compression
{
    public class ZipUtility
    {
        /// <param name="files">each file will be compressed idependently</param>
        /// <param name="maxDOP">max degree of parallelism</param>
        /// <param name="eachFileMaxSize">each file will be compressed and then chunked based on this parameter. eachFileMaxSize is in MB</param>
        /// <param name="path">location to store compressed files</param>
        /// <param name="password">compressed files can also become password protected</param>
        /// <param name="compressionLevel">It is up to you to choose between best speed to high compression</param>
        /// <param name="encryption">compressed files can also become encrypted too!</param>
        public async Task CompressAsync(
            FileInfo[] files,
            string path,
            int maxDOP = 1,
            int eachFileMaxSize = 5,
            string password = "",
            CompressionLevel compressionLevel = CompressionLevel.BestSpeed,
            EncryptionAlgorithm encryption = EncryptionAlgorithm.None
            )
        {
            if (eachFileMaxSize < 1)
                eachFileMaxSize = 5;

            var allTasks = new List<Task>();

            var threadCount = files.Length < maxDOP ? files.Length : maxDOP;
            var throttler = new SemaphoreSlim(threadCount);

            foreach (var file in files)
            {
                await throttler.WaitAsync();
                allTasks.Add(Task.Factory.StartNew(() =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.Name);

                    using ZipFile zip = new ZipFile();

                    if (!string.IsNullOrWhiteSpace(password))
                        zip.Password = password;

                    zip.BufferSize = eachFileMaxSize * 1024 * 1024;
                    zip.MaxOutputSegmentSize64 = eachFileMaxSize * 1024 * 1024;
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    zip.Encryption = encryption;
                    zip.CompressionLevel = compressionLevel;

                    zip.AddFile(file.FullName);
                    zip.Save(Path.Combine(path, $"{file.Name}.zip"));

                    throttler.Release();
                }));
            }

            Task.WaitAll(allTasks.ToArray());
        }

        /// <param name="files">list of files to extract</param>
        /// <param name="maxDOP">max degree of parallelism</param>
        /// <param name="path">location to extract the files</param>
        /// <param name="password">password used to extract the compressed files</param>
        public async Task DecompressAsync(
            FileInfo[] files,
            string path,
            int maxDOP = 1,
            string password = "")
        {
            var allTasks = new List<Task>();
            var threadCount = files.Length < maxDOP ? files.Length : maxDOP;
            var throttler = new SemaphoreSlim(threadCount);

            foreach (var file in files)
            {
                await throttler.WaitAsync();

                allTasks.Add(Task.Factory.StartNew(() =>
                {
                    using ZipFile zip1 = ZipFile.Read(file.FullName);
                    zip1.FlattenFoldersOnExtract = true;
                    foreach (ZipEntry e in zip1)
                    {
                        if (string.IsNullOrWhiteSpace(password))
                        {
                            e.Extract(path, ExtractExistingFileAction.OverwriteSilently);
                        }
                        else
                            e.ExtractWithPassword(path, ExtractExistingFileAction.OverwriteSilently, password);

                    }
                    throttler.Release();
                }));
            }
            Task.WaitAll(allTasks.ToArray());
        }
    }
}
