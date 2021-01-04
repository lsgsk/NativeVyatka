using Newtonsoft.Json;

namespace NativeVyatka
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
        public SigninApiUser user { get; set; }

        public class SigninApiUser
        {
            public string uid { get; set; }
            public string hostname { get; set; }
            public Roles roles { get; set; }
            public int cache { get; set; }
            public long timestamp { get; set; }
        }

        public class Roles
        {
            [JsonProperty(PropertyName = "1")]
            public string Name { get; set;}
        }
    }

    public class TokenApi
    {
        public string Token { get; set; }
    }
}

