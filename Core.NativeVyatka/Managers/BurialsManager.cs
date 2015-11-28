using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using System;
using System.Threading;
using System.Diagnostics;

namespace NativeVyatkaCore
{
    public class BurialsManager : IBurialsManager
    {
        public BurialsManager()
        {   
            mDataBase.DeleteAllBurial();
            for (int i = 0; i < 10; i++)
            {
                var item = new BurialEntity() { BirthTime = DateTime.Now, DeathTime = DateTime.Now, Desctiption = "Description" + i, HashId = Guid.NewGuid().ToString(), IsSended = i % 3 == 0, Latitude = 10, Longitude = 10, Name = "Name" + 1 };
                mDataBase.InsertOrReplaceBurial(item).Wait();
            }
        }

        public async Task<List<BurialEntity>> GetAllBurials(CancellationToken token)
        {
            try
            {
                var result = await mDataBase.GetAllBurial();
                token.ThrowIfCancellationRequested();
                return result;
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex);
                return new List<BurialEntity>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public async Task<BurialEntity> GetBurial(int id, CancellationToken token)
        {
            var result = await mDataBase.GetBurial(id);
            token.ThrowIfCancellationRequested();
            return result;
        }

        //синхранизировать неотправленные

        public Task<bool> InsertBurial(BurialEntity item)
        {
            return null;
        }

        public Task<bool> DeleteBurial(BurialEntity item)
        {
            return null;
        }

        public Task<bool> UpdateBurial(BurialEntity item)
        {
            return null;
        }

        [Dependency]
        private IDatabase mDataBase { get; set; }
    }
}

