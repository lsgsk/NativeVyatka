namespace Abstractions.Models.Network.ServiceEntities
{
    public class ApiProfile
    {
        public string sessid { get; set; }
        public string session_name { get; set; }
        public string token { get; set; }
        public ApiUser user { get; set; }        
    }
    public class ApiUser
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public long login { get; set; }
        public string status { get; set; }
        public ApiPicture picture { get; set; }
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

