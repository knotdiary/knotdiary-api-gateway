using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using KnotDiary.Common;
using KnotDiary.Models;
using System.Threading.Tasks;

namespace KnotDiary.Services
{
    public class StorageService : IStorageService
    {
        private string Endpoint { get; set; }

        private string AccountName { get; set; }

        private string AccountKey { get; set; }

        private string ConnectionString { get; set; }
        
        public StorageService(IConfigurationHelper configurationHelper, IOptions<StorageConfiguration> storageConfiguration)
        {
            Endpoint = configurationHelper.GetAppSettings("App:Storage:Endpoint");
            AccountName = configurationHelper.GetAppSettings("App:Storage:Name");
            AccountKey = configurationHelper.GetAppSettings("App:Storage:Key");

            var connectionString = configurationHelper.GetAppSettings("App:Storage:ConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = storageConfiguration.Value.StorageConnectionString;
            }

            ConnectionString = connectionString;
        }

        public async Task<string> UploadStreamAsync(string containerPath, MediaUpload media)
        {
            var blockBlob = await GetBlockBlobAsync(containerPath, media.FileName);
            
            media.File.Position = 0;
            await blockBlob.UploadFromStreamAsync(media.File);

            return blockBlob.StorageUri?.PrimaryUri.AbsoluteUri;
        }
        
        private async Task<CloudBlobContainer> GetContainerAsync(string containerPath)
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerPath);

            await blobContainer.CreateIfNotExistsAsync();
            return blobContainer;
        }

        private async Task<CloudBlockBlob> GetBlockBlobAsync(string containerPath, string blobName)
        {
            var blobContainer = await GetContainerAsync(containerPath);
            var blockBlob = blobContainer.GetBlockBlobReference(blobName);

            return blockBlob;
        }
    }
}
