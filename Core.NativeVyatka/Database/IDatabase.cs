using System.Threading.Tasks;
using System.Collections.Generic;

namespace NativeVyatkaCore
{
    public interface IDatabase
    {
        Task<int> BurialCount();
        Task<List<BurialEntity>> GetAllBurial();
        Task<List<BurialEntity>> GetAllUnsendedBurial();
        Task<BurialEntity> GetBurial(int id);      
        Task<int> InsertOrReplaceBurial(BurialEntity item);
        Task<int> DeleteBurial(BurialEntity item);
        Task DeleteAllBurial();
    }
}

