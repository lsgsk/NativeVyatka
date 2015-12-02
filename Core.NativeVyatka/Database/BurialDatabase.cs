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

        public Task<int> BurialCountAsync()
        {
            lock (locker)
            {
                return Table<BurialEntity>().CountAsync();
            }
        }

        public Task<List<BurialEntity>> GetAllBurialAsync()
        {
            lock (locker)
            {
                return Table<BurialEntity>().ToListAsync();
            }
        }

        public Task<List<BurialEntity>> GetAllUnsendedBurialAsync()
        {
            lock (locker)
            {
                return Table<BurialEntity>().Where(x => !x.IsSended).ToListAsync();
            }
        }

        public Task<BurialEntity> GetBurialAsync(int id)
        {
            lock (locker)
            {
                return Table<BurialEntity>().Where(x => x.Id == id).FirstOrDefaultAsync();
            }
        }

        public Task<int> InsertBurialAsync(BurialEntity item)
        {
            lock (locker)
            {
                return InsertAsync(item);
            }
        }

        public Task<int> InsertOrReplaceBurialAsync(BurialEntity item)
        {
            lock (locker)
            {
                return InsertOrReplaceAsync(item);
            }
        }

        public Task<int> DeleteBurialAsync(BurialEntity item)
        {
            lock (locker)
            {
                return DeleteAsync(item);
            }
        }

        public Task DeleteAllBurialAsync()
        {
            lock (locker)
            {
                return ExecuteAsync(string.Format("DELETE FROM {0}", BurialEntity.TableName));
            }
        }
        private readonly object locker = new object();
    }
}

