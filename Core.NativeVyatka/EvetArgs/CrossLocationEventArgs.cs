using System;

namespace NativeVyatkaCore
{
    public class CrossLocationEventArgs : EventArgs
    {
        public CrossLocationEventArgs(CrossLocation location)
        {
            this.Location = location;
        }
        public CrossLocation Location { get; set;}
    }
}

