namespace Core.Services.Storage.Cloud
{
    public class CloudStorageOptions
    {
        public const string SectionName = "CloudStorage";

        public string BucketName { get; init; }
    }
}
