using SQLite;

namespace NativeVyatka
{
    [Table(DbVersionTableName)]
    public class DbVersionEntity
    {
        [PrimaryKey]
        public int Value { get; set; }

        public const string DbVersionTableName = "Version";
    }
}
