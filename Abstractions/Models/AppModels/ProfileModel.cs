﻿using Abstractions.Models.DatabaseModels;
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
            this.Uid = entity.uid;
            this.Name = entity.name;
            this.Email = entity.mail;
            this.PictureUrl = entity.Picture.url;
        }

        public ProfileModel(ProfileEntity entity)
        {
            this.Uid = entity.Uid;
            this.Name = entity.Name;
            this.Email = entity.Email;
            this.PictureUrl = entity.PictureUrl;
        }

        public ProfileEntity ToProfileEntity()
        {
            return new ProfileEntity()
            {
                Uid = this.Uid,
                Name = this.Name,
                Email = this.Email,
                PictureUrl = this.PictureUrl
            };
        }

        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PictureUrl { get; set; }

        public static ProfileModel Null = new ProfileModel();
    }

}