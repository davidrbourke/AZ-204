using Azure.Storage;

using Azure.Storage.Sas;
using System;

namespace StorageAccountApp
{
    public class GenerateSaSas
    {
        private string _key;
        private readonly string _containerName = "testcontainer";
        private readonly string _storageAccName = "stgdrbgeneral";

        public GenerateSaSas(string key)
        {
            _key = key;
        }

        public Uri GenerateSasUrl()
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = "testcontainer",
                //BlobName = "MyProfilePic.jpg", // Enable for acces only to this file, otherwise access given to all blobs in container
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2)
            };

            sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            var sharedKeyCredential = new StorageSharedKeyCredential(_storageAccName, _key);

            string token = sasBuilder.ToSasQueryParameters(sharedKeyCredential).ToString();

            var uri = new UriBuilder
            {
                Scheme = "https",
                Host = $"{_storageAccName}.blob.core.windows.net",
                Path = $"{_containerName}/MyProfilePic.jpg",
                Query = token
            };

            return uri.Uri;
        }

    }
}
