using System;

namespace NativeVyatka
{
    public interface IVersionStorage
    {
        int GetVersion();
    }

    public sealed partial class BurialDatabase: IVersionStorage
    {
        public int GetVersion() {
            try {
                using var conn = GetConnection();
                var version = conn.Table<DbVersionEntity>().FirstOrDefault()?.Value ?? 0;
                return version;
            }
            catch (Exception ex) {
                iConsole.Error(ex);
            }
            return 0;
        }
    }
}
