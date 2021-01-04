using System;
using SQLite;

namespace NativeVyatka
{
    [Table(BurialTableName)]
    public class BurialEntity
    {     
        [PrimaryKey]
        public string CloudId { get; set;}
        public string UserHash { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Description { get; set; }
        public string BirthDay { get; set; }
        public string DeathDay { get; set; }
        public DateTime RecordTime { get; set; }        
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public double Altitude { get; set; }
        public double? Heading { get; set; }
        public string PicturePath { get; set; }
        public string PreviewPictureUrl { get; set; }
        public bool Favorite { get; set; }
        public bool Updated { get; set; }
        public bool Uploaded { get; set; }

        public const string BurialTableName = "Burial";
    } 
}
