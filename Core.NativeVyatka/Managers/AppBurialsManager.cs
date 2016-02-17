using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;
using Abstractions;

namespace NativeVyatkaCore
{
    public class AppBurialsManager : IBurialsManager
    {
        public AppBurialsManager(IDatabase dataBase)
        {   
            this.mDataBase = dataBase;
        }

        public async Task<List<BurialEntity>> GetAllBurials(CancellationToken? token = null)
        {
            try
            {
                var result = await mDataBase.GetAllBurialAsync();
                //token.ThrowIfCancellationRequested();
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

        public async Task<List<BurialEntity>> GetUnsendedBurials(CancellationToken? token = null)
        {
            try
            {
                var result = await mDataBase.GetAllUnsendedBurialAsync();
                //token.ThrowIfCancellationRequested();
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

        public async Task<BurialEntity> GetBurial(int id, CancellationToken? token = null)
        {
            try
            {
                var result = await mDataBase.GetBurialAsync(id);
                //token.ThrowIfCancellationRequested();
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

        public async Task InsertBurial(BurialEntity item, CancellationToken? token = null)
        {
            try
            {
                await mDataBase.InsertBurialAsync(item);
                //token.ThrowIfCancellationRequested();
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

        public async Task UpdateSendedBurial(List<BurialEntity> items, CancellationToken? token = null)
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

        public async Task DeleteBurial(BurialEntity item, CancellationToken? token = null)
        {
            try
            {
                await mDataBase.DeleteBurialAsync(item);
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

        public async Task UpdateBurial(BurialEntity item, CancellationToken? token = null)
        {
            try
            {
                await mDataBase.InsertOrReplaceBurialAsync(item);
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

        public IDatabase mDataBase { get; set; }
    }
}

