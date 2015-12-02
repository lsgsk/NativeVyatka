using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace NativeVyatkaCore
{
    public interface IBurialsManager
    {
        Task<List<BurialEntity>> GetAllBurials(CancellationToken token);
        Task<BurialEntity> GetBurial(int id, CancellationToken token);
        Task InsertBurial(string imagepath, CrossLocation location);
        Task InsertBurial(BurialEntity item);
        Task DeleteBurial(BurialEntity item);
        Task UpdateBurial(BurialEntity item);
    }
}
