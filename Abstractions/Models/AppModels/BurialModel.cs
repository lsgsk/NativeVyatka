using Abstractions.Exceptions;
using Abstractions.Models.DatabaseModels;
using System;
using System.Threading.Tasks;

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

        public async Task<ApiBurial> ToApiBurial(IBurialImageGuide guide)
        {


            return new ApiBurial()
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
                Picture = await ReadImage(guide, this.PicturePath)
            };
        }

        private async Task<byte[]> ReadImage(IBurialImageGuide guide, string path)
        {
            try
            {
                return await guide.LoadFromFileSystemAsync(this.PicturePath);
            }
            catch (FileGuideException)
            {
                return new byte[0];
            }
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
