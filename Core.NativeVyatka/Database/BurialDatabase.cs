using SQLite;
using Abstractions;
using Abstractions.Models.DatabaseModels;
using System.IO;
using NativeVyatkaCore.Utilities;
using Abstractions.Interfaces.Settings;

namespace NativeVyatkaCore.Database
{
    public sealed partial class BurialDatabase : IDataStorage
    {
        public static void InitILobbyPhoneDatabase(string path)
        {
            mPathToDatabase = Path.Combine(path, DbName);
            using (var conn = GetConnection())
            {
                conn.CreateTable<DbVersionEntity>();
                var v = conn.Table<DbVersionEntity>().FirstOrDefault();
                mVersion = v?.Value ?? 0;
                if (mVersion == 0)
                {
                    iConsole.WriteLine("(^_^)");
                }
                conn.CreateTable<BurialEntity>();
                conn.CreateTable<ProfileEntity>();
            }
        }

        public BurialDatabase(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        private static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(mPathToDatabase);
        }

        public void ClearDataBase()
        {
            using (var conn = GetConnection())
            {
                conn.DeleteAll<BurialEntity>();
                conn.DeleteAll<ProfileEntity>();
            }
        }           

        private const string DbName = "burials.db";
        private static string mPathToDatabase;
        private static int mVersion = 0;
        private readonly ISettingsProvider settingsProvider;
    }
}

