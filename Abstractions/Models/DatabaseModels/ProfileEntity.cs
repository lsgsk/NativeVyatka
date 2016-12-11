using SQLite;

namespace Abstractions.Models.DatabaseModels
{
    [Table(ProfileEntity.ProfileTableName)]
    public class ProfileEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }

        public const string ProfileTableName = "Profile";
    }
}
