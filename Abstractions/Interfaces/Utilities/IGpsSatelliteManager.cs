using System;

namespace Abstractions.Interfaces.Utilities
{
    public interface IGpsSatelliteManager
    {
        event EventHandler<int> OnGpsEnableChanged;
    }
}
