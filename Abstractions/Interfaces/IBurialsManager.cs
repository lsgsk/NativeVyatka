using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Abstractions
{
    public interface IBurialsManager
    {
        Task<List<BurialEntity>> GetAllBurials(CancellationToken? token = null);
        Task<List<BurialEntity>> GetUnsendedBurials(CancellationToken? token = null);
        Task<BurialEntity> GetBurial(int id, CancellationToken? token = null);
        Task InsertBurial(BurialEntity item, CancellationToken? token = null);
        Task UpdateSendedBurial(List<BurialEntity> item, CancellationToken? token = null);
        Task DeleteBurial(BurialEntity item, CancellationToken? token = null);
        Task UpdateBurial(BurialEntity item, CancellationToken? token = null);
    }
}
