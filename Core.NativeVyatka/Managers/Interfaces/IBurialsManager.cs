using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace NativeVyatkaCore
{
    public interface IBurialsManager
    {
        Task<List<BurialEntity>> GetAllBurials(CancellationToken token);
        Task<BurialEntity> GetBurial(int id, CancellationToken token);
        Task<bool> InsertBurial(BurialEntity item);
        Task<bool> DeleteBurial(BurialEntity item);
        Task<bool> UpdateBurial(BurialEntity item);
    }
}
