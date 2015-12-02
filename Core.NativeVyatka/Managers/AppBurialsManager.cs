using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using System;
using System.Threading;
using System.Diagnostics;

namespace NativeVyatkaCore
{
    public class AppBurialsManager : IBurialsManager
    {
        public AppBurialsManager(IDatabase dataBase)
        {   
            this.mDataBase = dataBase;
            /*#if DEBUG        mDataBase.DeleteAllBurialAsync();            for (int i = 0; i < 10; i++)            {                var item = new BurialEntity() { BirthTime = DateTime.Now, DeathTime = DateTime.Now, Desctiption = "Description" + i, HashId = Guid.NewGuid().ToString(), IsSended = i % 3 == 0, Latitude = 10, Longitude = 10, Name = "Name" + 1 };                mDataBase.InsertBurialAsync(item).Wait();            }            #endif*/
        }

        public async Task<List<BurialEntity>> GetAllBurials(CancellationToken token)
        {
            try
            {
                var result = await mDataBase.GetAllBurialAsync();
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

        public async Task<List<BurialEntity>> GetUnsendedBurials(CancellationToken token)
        {
            try
            {
                var result = await mDataBase.GetAllUnsendedBurialAsync();
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
            try
            {
                var result = await mDataBase.GetBurialAsync(id);
                token.ThrowIfCancellationRequested();
                return result;
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public async Task InsertBurial(BurialEntity item, CancellationToken token)
        {
            try
            {
                await mDataBase.InsertOrReplaceBurialAsync(item);
                token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public async Task UpdateSendedBurial(List<BurialEntity> items, CancellationToken token)
        {
            try
            {
                await mDataBase.UpdateAllBurialAsync(items);
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public Task DeleteBurial(BurialEntity item, CancellationToken token)
        {
            return null;
        }

        public Task UpdateBurial(BurialEntity item, CancellationToken token)
        {
            return null;
        }

        public IDatabase mDataBase { get; set; }
    }
}

