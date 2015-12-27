﻿using System.Threading.Tasks;
using System.Collections.Generic;

namespace Abstractions
{
    public interface IDatabase
    {
        Task<int> BurialCountAsync();
        Task<List<BurialEntity>> GetAllBurialAsync();
        Task<List<BurialEntity>> GetAllUnsendedBurialAsync();
        Task<BurialEntity> GetBurialAsync(int id);      
        Task<int> InsertBurialAsync(BurialEntity item);
        Task<int> InsertOrReplaceBurialAsync(BurialEntity item);
        Task<int> UpdateAllBurialAsync(IEnumerable<BurialEntity> items);
        Task<int> DeleteBurialAsync(BurialEntity item);
        Task DeleteAllBurialAsync();
    }
}

