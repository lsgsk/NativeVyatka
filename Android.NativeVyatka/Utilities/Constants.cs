using System;

namespace NativeVyatkaAndroid
{
    public enum ActivityActions
    {
        TAKE_PHOTO = 0,
        OPEN_BURIAL = 1,
        RETAKE_PHOTO = 2,
    }


    public static class Constants
    {
        public const string BURIAL_ID = "burial_id";
        public const string BURIAL_RESULT_MESSAGE = "burial_activity_message";
    }
}

