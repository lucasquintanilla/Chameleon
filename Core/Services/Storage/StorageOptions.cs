using Core.Services.Storage.Cloud;
using Core.Services.Storage.Local;

namespace Core.Services.Storage
{
    internal class StorageOptions
    {
        public const string SectionName = "Storage";
        public CloudStorageOptions Cloud { get; set; }
        public LocalStorageOptions Local { get; set; }
    }
}
