using System;
using SQLite;

namespace Abstractions.Models.DatabaseModels
{
    [Table(BurialTableName)]
    public class BurialEntity
    {     
        [PrimaryKey]
        public string CloudId { get; set;}
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Desctiption { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? DeathDay { get; set; }
        public DateTime RecordTime { get; set; }        
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Heading { get; set; }
        public string PicturePath { get; set; }
        public bool Updated { get; set; }
        public bool Uploaded { get; set; }

        public const string BurialTableName = "Burial";
    } 
}
