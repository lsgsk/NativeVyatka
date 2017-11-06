using Abstractions.Models.DatabaseModels;
using System;

namespace Abstractions.Models.AppModels
{
    public class BurialModel
    {
        public string CloudId { get; set; }
        public BurialStatus Status { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? DeathDay { get; set; }
        public DateTime RecordTime { get; set; }
        public Position Location { get; set; }
        public string PicturePath { get; set; }
        public bool Updated { get; set; }
        public bool Uploaded { get; set; }

        public BurialModel()
        {
            Name = Surname = Patronymic = Description = string.Empty;
            this.CloudId = Guid.NewGuid().ToString();
            this.Location = new Position();
        }
    
        public BurialModel(BurialEntity entity)
        {
            this.CloudId = entity.CloudId;
            this.Status = BurialStatus.Normal;
            this.Name = entity.Name;
            this.Surname = entity.Surname;
            this.Patronymic = entity.Patronymic;
            this.Description = entity.Desctiption;
            this.BirthDay = (entity.BirthDay != null) ? DateTime.SpecifyKind(entity.BirthDay.Value, DateTimeKind.Utc) : (DateTime?)null;
            this.DeathDay = (entity.DeathDay != null) ? DateTime.SpecifyKind(entity.DeathDay.Value, DateTimeKind.Utc) : (DateTime?)null;
            this.RecordTime = DateTime.SpecifyKind(entity.RecordTime, DateTimeKind.Utc);
            this.Location = new Position()
            {
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Altitude = entity.Altitude,
                Heading = entity.Heading
            };           
            this.PicturePath = entity.PicturePath;
            this.Updated = entity.Updated;
            this.Uploaded = entity.Uploaded;
        }

        public BurialEntity ToBurialEntity()
        {
            return new BurialEntity()
            {
                CloudId = this.CloudId,
                Name = this.Name,
                Surname = this.Surname,
                Patronymic = this.Patronymic,
                Desctiption = this.Description,
                BirthDay = this.BirthDay,
                DeathDay = this.DeathDay,
                RecordTime = this.RecordTime,
                Latitude = this.Location.Latitude,
                Longitude = this.Location.Longitude,
                Altitude = this.Location.Altitude,
                Heading = this.Location.Heading,
                PicturePath = this.PicturePath,
                Updated = this.Updated,
                Uploaded = this.Uploaded
            };
        }

        public ApiBurialToSend ToApiBurial()
        {
            return new ApiBurialToSend()
            {
                CloudId = this.CloudId,
                Status = 1,
                Name = this.Name,
                Surname = this.Surname,
                Patronymic = this.Patronymic,
                Description = this.Description,
                BirthDay = this.BirthDay?.ToString("dd-MM-yyyy"),
                DeathDay = this.DeathDay?.ToString("dd-MM-yyyy"),
                RecordTime = (Int32)(this.RecordTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                Latitude = this.Location.Latitude,
                Longitude = this.Location.Longitude,
                Altitude = this.Location.Altitude,
                Heading = this.Location.Heading,
                Picture = null
            };
        }

        public BurialModel(ApiBurialToReceive entity)
        {
            this.CloudId = entity.CloudId;
            this.Status = (BurialStatus)entity.Status;
            this.Name = entity.Name;
            this.Surname = entity.Surname;
            this.Patronymic = entity.Patronymic;
            this.Description = entity.Description;
            if (entity.BirthDay != "00-00-0000")
            {
                this.BirthDay = DateTime.Parse(entity.BirthDay);
            }
            if (entity.DeathDay != "00-00-0000")
            {
                this.DeathDay = DateTime.Parse(entity.DeathDay);
            }
            this.RecordTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(entity.RecordTime);
            this.Location = new Position()
            {
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Altitude = entity.Altitude,
                Heading = entity.Heading
            };
            //здесь фактически устанавливается урл 
            this.PicturePath = entity.PictureUrl;
            this.Updated = true;
            this.Uploaded = true;
        }

        public class Position
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Altitude { get; set; }
            public double? Heading { get; set; }
        }

        public enum BurialStatus
        {            
            ToRemove = 0,
            Normal = 1,
        }
        public static BurialModel Null = new BurialModel();
    }
}
