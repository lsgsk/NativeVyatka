using System;

namespace NativeVyatkaCore
{
    public interface ILocationManager : IDisposable
    {
        bool GpsStatus { get; set; }
        CrossLocation Location { get; set; }
        event EventHandler<CrossLocationEventArgs> OnLocationChanged;
        event EventHandler<bool> OnGpsStatusChanged;
    }
}

