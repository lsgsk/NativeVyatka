using SQLite;
using System.IO;

namespace NativeVyatka
{
    public interface IDataStorage
    {
        void ClearDataBase();
    }

    public sealed partial class BurialDatabase : IDataStorage
    {
        private const string DbName = "burials.db";
        private static string pathToDatabase;

        public static void InitPhoneDatabase(string path)
        {
            pathToDatabase = Path.Combine(path, DbName);
            using var conn = GetConnection();
            conn.CreateTable<DbVersionEntity>();
            conn.CreateTable<BurialEntity>();
            conn.CreateTable<ProfileEntity>();
        }

        public BurialDatabase() { }

        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(pathToDatabase);
        }

        public void ClearDataBase()
        {
            using var conn = GetConnection();
            conn.DeleteAll<BurialEntity>();
            conn.DeleteAll<ProfileEntity>();
        }
    }
}

