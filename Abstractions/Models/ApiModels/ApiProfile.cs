namespace Abstractions.Models.Network.ServiceEntities
{
    public class ApiProfile
    {
        public string Sessid { get; set; }
        public string Session_name { get; set; }
        public string Token { get; set; }
        public ApiUser User { get; set; }        
    }
    public class ApiUser
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public long login { get; set; }
        public string Status { get; set; }
        public string Timezone { get; set; }
        public string Language { get; set; }
        public ApiPicture Picture { get; set; }
    }
    public class ApiPicture
    {
        public string fid { get; set; }
        public string uid { get; set; }
        public string filename { get; set; }
        public string uri { get; set; }
        public string url { get; set; }
    }
}

