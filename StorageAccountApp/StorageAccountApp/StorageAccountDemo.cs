using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StorageAccountApp
{
    public class StorageAccountDemo
    {
        private BlobServiceClient _client;
        
        private static readonly string _downloadPath = "D:\\dev\\AZ-204\\blobs";
        public StorageAccountDemo(string connStr)
        {

            _client = new BlobServiceClient(connStr);
        }

        public async Task DownloadAllBlobsInContainer()
        {
            var containerClients = _client.GetBlobContainers();

            foreach (BlobContainerItem container in containerClients)
            {
                var containerClient = _client.GetBlobContainerClient(container.Name);
                Console.WriteLine(container.Name);
                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    Console.WriteLine(blobItem.Name);

                    Console.WriteLine($"Downloading blob to: {_downloadPath}");

                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadInfo blobDownloadInfo = blobClient.Download();

                    // Access blob properties
                    BlobProperties props = blobClient.GetProperties();
                    Console.WriteLine("Properties");
                    Console.WriteLine($"Access tier: {props.AccessTier}");
                    Console.WriteLine($"Blob expires on: {props.ExpiresOn.UtcDateTime.ToLongDateString()}");

                    // Access blob metadata
                    Console.WriteLine("Metadata keyvalue pair");
                    foreach (var metadata in props.Metadata)
                    {
                        Console.WriteLine($"{metadata.Key}: {metadata.Value}");
                    }

                    using (FileStream fs = File.OpenWrite($"{_downloadPath}\\{blobItem.Name}"))
                    {
                        blobDownloadInfo.Content.CopyTo(fs);
                        fs.Close();
                    }
                }
            }
        }

        public async Task UploadToContainer()
        {
            Console.WriteLine("Uploading");
            
            var uploadPath = $"{_downloadPath}\\toUpload";
            var uploadContainerName = "testcontainer";
            var sampleUploadFileName = "Sample upload.txt";

            BlobContainerClient uploadContainer = _client.GetBlobContainerClient(uploadContainerName);

            using FileStream fsu = File.OpenRead($"{uploadPath}\\{sampleUploadFileName}");
            
            // Upload file
            BlobContentInfo blobContentInfo = await uploadContainer.UploadBlobAsync(sampleUploadFileName, fsu);

            // Update metadata
            BlobClient metaBlobClient = uploadContainer.GetBlobClient("Sample upload.txt");
            IDictionary<string, string> newMetadata = new Dictionary<string, string>();
            newMetadata.Add("Owner", "David");

            await metaBlobClient.SetMetadataAsync(newMetadata);
        }
    }
}
