using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.Services;

public class StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;

    public StorageService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task PutObject(Stream stream)
    {
        var request = new PutObjectRequest()
        {
            BucketName = "post-attachments",
            Key = Guid.NewGuid() + ".jpg",
            InputStream = stream,
            ContentType = "image/jpg"
        };
        await _s3Client.PutObjectAsync(request);
    }
}

public interface IStorageService
{
    Task PutObject(Stream stream);
}