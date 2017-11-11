namespace Abstractions.Models.Network.ServiceEntities
{
    public class LoginApiProfile
    {
        public string sessid { get; set; }
        public string session_name { get; set; }
        public string token { get; set; }
        public LoginApiUser user { get; set; }

        public class LoginApiUser
        {
            public string uid { get; set; }
            public string name { get; set; }
            public string mail { get; set; }
            public LoginApiPicture picture { get; set; }
        }
        public class LoginApiPicture
        {
            public string url { get; set; }
        }
    }

    public class SigninApiProfile
    {
        public string sessid { get; set; }
        public string session_name { get; set; }
    }
}

