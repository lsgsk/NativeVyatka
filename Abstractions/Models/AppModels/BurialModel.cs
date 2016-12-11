using Abstractions.Models.DatabaseModels;
using System;

namespace Abstractions.Models.AppModels
{
    public class BurialModel
    {
        public string CloudId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Desctiption { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? DeathDay { get; set; }
        public DateTime RecordTime { get; set; }
        public Position Location { get; set; }
        public string PicturePath { get; set; }
        public bool Updated { get; set; }

        public BurialModel()
        {
            this.CloudId = Guid.NewGuid().ToString();
            this.Location = new Position();
        }

        public BurialModel(BurialEntity entity)
        {
            this.CloudId = entity.CloudId;
            this.Name = entity.Name;
            this.Surname = entity.Surname;
            this.Patronymic = entity.Patronymic;
            this.Desctiption = entity.Desctiption;
            this.BirthDay = entity.BirthDay;
            this.DeathDay = entity.DeathDay;            
            this.RecordTime = entity.RecordTime;
            this.Location.Latitude = entity.Latitude;
            this.Location.Longitude = entity.Longitude;
            this.Location.Altitude = entity.Altitude;
            this.Location.Heading = entity.Heading;
            this.PicturePath = entity.PicturePath;
            this.Updated = entity.Updated;
        }

        public BurialEntity ToBurialEntity()
        {
            return new BurialEntity()
            {
                CloudId = this.CloudId,
                Name = this.Name,
                Surname = this.Surname,
                Patronymic = this.Patronymic,
                Desctiption = this.Desctiption,
                BirthDay = this.BirthDay,
                DeathDay = this.DeathDay,
                RecordTime = this.RecordTime,
                Latitude = this.Location.Latitude,
                Longitude = this.Location.Longitude,
                Altitude = this.Location.Altitude,
                Heading = this.Location.Heading,
                PicturePath = this.PicturePath,
                Updated = this.Updated,
            };
        }

        public class Position
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Altitude { get; set; }
            public double Heading { get; set; }
        }

        public static BurialModel Null = new BurialModel();
    }
}
