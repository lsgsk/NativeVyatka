using SQLite;

namespace NativeVyatka
{
    [Table(ProfileTableName)]
    public class ProfileEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }

        public const string ProfileTableName = "Profile";
    }
}
