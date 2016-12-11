using Abstractions.Models.DatabaseModels;
using Abstractions.Models.Network.ServiceEntities;

namespace Abstractions.Models.AppModels
{
    public class ProfileModel
    {
        public ProfileModel()
        {
        }

        public ProfileModel(ApiUser entity)
        {
            this.Name = entity.name;
            this.Email = entity.mail;
            this.PictureUrl = entity.Picture.url;
        }

        public ProfileModel(ProfileEntity entity)
        {

        }

        public ProfileEntity ToProfileEntity()
        {
            return new ProfileEntity()
            {
                Name = this.Name,
                Email = this.Email,
                PictureUrl = this.PictureUrl
            };
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }

        public static ProfileModel Null = new ProfileModel();
    }

}
