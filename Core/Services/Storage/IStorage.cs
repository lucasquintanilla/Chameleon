using Core.Services.Storage.Models;
using System.Threading.Tasks;

namespace Core.Services.Storage
{
    public interface IStorage<T>
    {
        Task Save(T obj);
    }

    public interface IStorage : IStorage<StorageObject> { }
}
