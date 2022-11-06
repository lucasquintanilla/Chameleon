using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;

namespace Core.Services.Storage;

public class CloudStorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;

    public CloudStorageService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task Save(StorageObject obj)
    {
        var request = new PutObjectRequest()
        {
            BucketName = "post-attachments",
            Key = obj.Key,
            InputStream = obj.Stream,
            ContentType = obj.ContentType
        };
        await _s3Client.PutObjectAsync(request);
    }
}

public interface IStorageService
{
    Task Save(StorageObject obj);
}