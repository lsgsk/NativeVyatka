using System;

namespace NativeVyatkaAndroid
{
    public class BaseEventArgs<T> : EventArgs
    {
        public BaseEventArgs()
        {            
        }
        public int Position { get; set; }
        public T Item  { get; set; }
    }
}

