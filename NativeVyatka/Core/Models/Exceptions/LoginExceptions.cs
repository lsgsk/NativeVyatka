using System;

namespace NativeVyatka
{
    public class NotValidLoginOrPasswordException : Exception
    {
        public string EmailMessage { get; set; }
        public string PasswordMessage { get; set; }
    }

    public class AuthorizationSyncException : Exception { }

    public class LoginException : Exception { }

    public class SessionException : Exception { }

    public class PermissionsException : Exception { }

    public class GeolocationException : Exception { }
}