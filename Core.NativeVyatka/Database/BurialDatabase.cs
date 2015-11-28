using SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace NativeVyatkaCore
{
    public class BurialDatabase : SQLiteAsyncConnection, IDatabase
    {
        public BurialDatabase(string path) : base(path)
        {
            CreateTableAsync<BurialEntity>().ContinueWith(t => { Debug.WriteLine("Burial table created!"); });
        }

        public Task<int> BurialCount()
        {
            lock (locker)
            {
                return Table<BurialEntity>().CountAsync();
            }
        }

        public Task<List<BurialEntity>> GetAllBurial()
        {
            lock (locker)
            {
                return Table<BurialEntity>().ToListAsync();
            }
        }

        public Task<List<BurialEntity>> GetAllUnsendedBurial()
        {
            lock (locker)
            {
                return Table<BurialEntity>().Where(x => !x.IsSended).ToListAsync();
            }
        }

        public Task<BurialEntity> GetBurial(int id)
        {
            lock (locker)
            {
                return Table<BurialEntity>().Where(x => x.Id == id).FirstOrDefaultAsync();
            }
        }

        public Task<int> InsertOrReplaceBurial(BurialEntity item)
        {
            lock (locker)
            {
                return InsertOrReplaceAsync(item);
            }
        }

        public Task<int> DeleteBurial(BurialEntity item)
        {
            lock (locker)
            {
                return DeleteAsync(item);
            }
        }

        public Task DeleteAllBurial()
        {
            lock (locker)
            {
                return ExecuteAsync(string.Format("DELETE FROM {0}", BurialEntity.TableName));
            }
        }
        private readonly object locker = new object();
    }
}

