using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace NativeVyatkaCore
{
    public interface IBurialsManager
    {
        Task<List<BurialEntity>> GetAllBurials(CancellationToken token);
        Task<List<BurialEntity>> GetUnsendedBurials(CancellationToken token);
        Task<BurialEntity> GetBurial(int id, CancellationToken token);
        Task InsertBurial(BurialEntity item, CancellationToken token);
        Task UpdateSendedBurial(List<BurialEntity> item, CancellationToken token);
        Task DeleteBurial(BurialEntity item, CancellationToken token);
        Task UpdateBurial(BurialEntity item, CancellationToken token);
    }
}
