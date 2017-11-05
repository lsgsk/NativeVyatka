using System;

namespace Abstractions
{
    public class ApiBurial
    {
        public string CloudId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Description { get; set; }
        public string BirthDay { get; set; }
        public string DeathDay { get; set; }
        public long RecordTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Heading { get; set; }
        public string Picture { get; set; }
    }
}

