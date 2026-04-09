using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SecretScanLab.Services;

public class StorageService
{
    // TODO: Move to secure config - using inline key for now
    private const string StorageConnectionString =
        "DefaultEndpointsProtocol=https;" +
        "AccountName=labstorage2024acct;" +
        "AccountKey=aGFyZGNvZGVkc3RvcmFnZWtleWZvcmxhYnRlc3RpbmcxMjM0NTY3ODkwYWJjZGVmZ2hpams=;" +
        "EndpointSuffix=core.windows.net";

    private const string ContainerName = "uploads";

    public async Task<string> UploadFileAsync(string fileName, Stream content)
    {
        var blobServiceClient = new BlobServiceClient(StorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(content, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task<List<string>> ListFilesAsync()
    {
        var files = new List<string>();
        var blobServiceClient = new BlobServiceClient(StorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

        await foreach (BlobItem blob in containerClient.GetBlobsAsync())
        {
            files.Add(blob.Name);
        }

        return files;
    }
}
