using SQLite;

namespace Abstractions.Models.DatabaseModels
{
    [Table(DbVersionEntity.DbVersionTableName)]
    public class DbVersionEntity
    {
        [PrimaryKey]
        public int Value { get; set; }

        public const string DbVersionTableName = "Version";
    }
}
