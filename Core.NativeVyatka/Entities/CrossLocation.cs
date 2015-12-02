using System;

namespace NativeVyatkaCore
{
    public class CrossLocation
    {
        public CrossLocation()
        {
        }

        public CrossLocation(double longitude, double latitude, float accuracy, long time)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.Accuracy = accuracy;
            this.Time = time;
        }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public float Accuracy { get; set; }

        public long Time { get; set; }
    }
}

