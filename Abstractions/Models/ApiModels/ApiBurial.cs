using System;
using System.Collections.Generic;

namespace Abstractions
{
    public class BurialCollection
    {
        public string Uid { get; set; }
        public List<ApiBurial> Colllection { get; set; }
    }

    public class ApiBurial
    {
        public string CloudId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Desctiption { get; set; }
        public DateTime? BirthDay { get; set; }
        public DateTime? DeathDay { get; set; }
        public DateTime RecordTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Heading { get; set; }
        public byte[] Picture { get; set; }
    }
}

