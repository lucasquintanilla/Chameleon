using System.IO;

namespace Core.Services.Storage;

public class StorageObject
{
    public string Key { get; set; }
    public string ContentType { get; set; }
    public Stream Stream { get; set; }
}
