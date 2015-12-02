using System.Threading.Tasks;
using System.Collections.Generic;

namespace NativeVyatkaCore
{
    public interface IDatabase
    {
        Task<int> BurialCountAsync();
        Task<List<BurialEntity>> GetAllBurialAsync();
        Task<List<BurialEntity>> GetAllUnsendedBurialAsync();
        Task<BurialEntity> GetBurialAsync(int id);      
        Task<int> InsertBurialAsync(BurialEntity item);
        Task<int> InsertOrReplaceBurialAsync(BurialEntity item);
        Task<int> DeleteBurialAsync(BurialEntity item);
        Task DeleteAllBurialAsync();
    }
}

