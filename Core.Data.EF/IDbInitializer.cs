using System.Threading.Tasks;

namespace Core.Data.EF
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}