using Amazon.S3;
using Amazon.S3.Model;
using Core.Services.Storage.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Core.Services.Storage.Cloud;

public class CloudStorage : IStorage
{
    private readonly CloudStorageOptions _options;
    private readonly IAmazonS3 _s3Client;

    public CloudStorage(IOptions<CloudStorageOptions> options, IAmazonS3 s3Client)
    {
        _options = options.Value;
        _s3Client = s3Client;
    }

    public async Task Save(StorageObject obj)
    {
        var request = new PutObjectRequest()
        {
            BucketName = _options.BucketName,
            Key = obj.Key,
            InputStream = obj.Content,
            ContentType = obj.ContentType
        };
        await _s3Client.PutObjectAsync(request);
    }
}
