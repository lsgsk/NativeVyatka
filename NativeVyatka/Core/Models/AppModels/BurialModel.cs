using System;

namespace NativeVyatka
{
    public class BurialModel : IEquatable<BurialModel>
    {
        public string CloudId { get; set; }
        public string UserHash { get; set; }
        public BurialStatus Status { get; private set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Description { get; set; }
        public string BirthDay { get; set; }
        public string DeathDay { get; set; }
        public DateTime RecordTime { get; set; }
        public Position Location { get; set; }
        public string PicturePath { get; set; }
        public string PreviewPictureUrl { get; set; }
        public bool Favorite { get; set; }
        public bool Updated { get; set; }
        public bool Uploaded { get; set; }

        public BurialModel() : this(string.Empty) {
        }

        public BurialModel(string userHash) {
            this.Name = Surname = Patronymic = Description = string.Empty;
            this.CloudId = Guid.NewGuid().ToString();
            this.UserHash = userHash;
            this.Location = new Position();
        }

        public BurialModel Copy() {
            var copy = new BurialModel {
                CloudId = this.CloudId,
                UserHash = this.UserHash,
                Status = this.Status,
                Surname = this.Surname,
                Name = this.Name,
                Patronymic = this.Patronymic,
                Description = this.Description,
                BirthDay = this.BirthDay,
                DeathDay = this.DeathDay,
                RecordTime = this.RecordTime,
                Location = new Position() {
                    Latitude = this.Location.Latitude,
                    Longitude = this.Location.Longitude,
                    Accuracy = this.Location.Accuracy,
                    Altitude = this.Location.Altitude,
                    Heading = this.Location.Heading
                },
                PicturePath = this.PicturePath,
                PreviewPictureUrl = this.PreviewPictureUrl,
                Favorite = this.Favorite,
                Updated = this.Updated,
                Uploaded = this.Uploaded
            };
            return copy;
        }

        public override bool Equals(object obj) {
            return Equals(obj as BurialModel);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public bool Equals(BurialModel other) {
            if (other is null) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return other.CloudId == this.CloudId &&
                   other.UserHash == this.UserHash &&
                   other.Status == this.Status &&
                   other.Surname == this.Surname &&
                   other.Name == this.Name &&
                   other.Patronymic == this.Patronymic &&
                   other.Description == this.Description &&
                   other.BirthDay == this.BirthDay &&
                   other.DeathDay == this.DeathDay &&
                   other.RecordTime == this.RecordTime &&
                   other.Location.Latitude == this.Location.Latitude &&
                   other.Location.Longitude == this.Location.Longitude &&
                   other.Location.Accuracy == this.Location.Accuracy &&
                   other.Location.Altitude == this.Location.Altitude &&
                   other.Location.Heading == this.Location.Heading &&
                   other.PicturePath == this.PicturePath &&
                   other.PreviewPictureUrl == this.PreviewPictureUrl &&
                   other.Favorite == this.Favorite &&
                   other.Updated == this.Updated &&
                   other.Uploaded == this.Uploaded;
        }

        public static bool operator ==(BurialModel lhs, BurialModel rhs) {
            if (lhs is null) {
                if (rhs is null) {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(BurialModel lhs, BurialModel rhs) {
            return !(lhs == rhs);
        }

        public BurialModel(BurialEntity entity) {
            this.CloudId = entity.CloudId;
            this.UserHash = entity.UserHash;
            this.Status = BurialStatus.Normal;
            this.Surname = entity.Surname ?? "";
            this.Name = entity.Name ?? "";
            this.Patronymic = entity.Patronymic ?? "";
            this.Description = entity.Description ?? "";
            this.BirthDay = entity.BirthDay;
            this.DeathDay = entity.DeathDay;
            this.RecordTime = DateTime.SpecifyKind(entity.RecordTime, DateTimeKind.Utc);
            this.Location = new Position() {
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Accuracy = entity.Accuracy,
                Altitude = entity.Altitude,
                Heading = entity.Heading
            };
            this.PicturePath = entity.PicturePath;
            this.PreviewPictureUrl = entity.PreviewPictureUrl;
            this.Favorite = entity.Favorite;
            this.Updated = entity.Updated;
            this.Uploaded = entity.Uploaded;
        }

        public BurialEntity ToBurialEntity() {
            return new BurialEntity() {
                CloudId = this.CloudId,
                UserHash = this.UserHash,
                Surname = this.Surname,
                Name = this.Name,
                Patronymic = this.Patronymic,
                Description = this.Description,
                BirthDay = this.BirthDay,
                DeathDay = this.DeathDay,
                RecordTime = this.RecordTime,
                Latitude = this.Location.Latitude,
                Longitude = this.Location.Longitude,
                Accuracy = this.Location.Accuracy,
                Altitude = this.Location.Altitude,
                Heading = this.Location.Heading,
                PicturePath = this.PicturePath,
                PreviewPictureUrl = this.PreviewPictureUrl,
                Favorite = this.Favorite,
                Updated = this.Updated,
                Uploaded = this.Uploaded
            };
        }

        public ApiBurialToSend ToApiBurial() {
            return new ApiBurialToSend() {
                CloudId = this.CloudId,
                Status = 1,
                Surname = this.Surname,
                Name = this.Name,
                Patronymic = this.Patronymic,
                Description = this.Description,
                BirthDay = this.BirthDay,
                DeathDay = this.DeathDay,
                RecordTime = (int)this.RecordTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                Latitude = this.Location.Latitude,
                Longitude = this.Location.Longitude,
                Accuracy = this.Location.Accuracy,
                Altitude = this.Location.Altitude,
                Heading = this.Location.Heading,
                Favorite = this.Favorite,
                Picture = null
            };
        }

        public BurialModel(ApiBurialToReceive entity, string userHash) {
            this.CloudId = entity.CloudId;
            this.UserHash = userHash;
            this.Status = (BurialStatus)entity.Status;
            this.Surname = entity.Surname;
            this.Name = entity.Name;
            this.Patronymic = entity.Patronymic;
            this.Description = entity.Description;
            this.BirthDay = entity.BirthDay;
            this.DeathDay = entity.DeathDay;
            this.RecordTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(entity.RecordTime);
            this.Location = new Position() {
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Accuracy = entity.Accuracy,
                Altitude = entity.Altitude,
                Heading = entity.Heading
            };
            //здесь фактически устанавливается урл 
            this.PicturePath = entity.PictureUrl;
            this.PreviewPictureUrl = entity.PreviewPictureUrl;
            this.Favorite = entity.Favorite;
            this.Updated = true;
            this.Uploaded = true;
        }

        public class Position
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Accuracy { get; set; }
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
