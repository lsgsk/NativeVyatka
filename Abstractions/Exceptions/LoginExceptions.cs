using System;

namespace Abstractions.Exceptions
{
    public class NotValidLoginOrPasswordException : Exception
    {
        public string EmailMessage { get; set; }
        public string PasswordMessage { get; set; }
    }

    public class AuthorizationSyncException : Exception
    {
    }

    public class LoginException : Exception
    {
    }   
}
