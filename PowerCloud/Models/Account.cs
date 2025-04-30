namespace PowerCloud.Models
{
    public class Account
    {
        private string pUserName;
        private string pUserNasLink;
        private string pAccessToken;
        private string pResfreshToken;
        public string UserName
        {
            get { return pUserName; }
            set { pUserName = value; }
        }

        public string UserNasLink
        {
            get { return pUserNasLink; }
            set { pUserNasLink = value; }
        }

        public string AccessToken
        {
            get { return pAccessToken; }
            set { pAccessToken = value; }
        }

        public string ResfreshToken
        {
            get { return pResfreshToken; }
            set { pResfreshToken = value; }
        }

        public string SystemInfoStr { get; set; }
        public string SortType { get; set; }

        public Account() : this("Unknown", "unknown", "unknown", "x.y.z", "1", "{\"HostName\":\"server.x.y.z\"}")
        {
        }

        public Account(Account copy)
        {
            UserNasLink = copy.UserNasLink;
            UserName = copy.UserName;
            AccessToken = copy.AccessToken;
            ResfreshToken = copy.ResfreshToken;
            SystemInfoStr = copy.SystemInfoStr;
            SortType = copy.SortType;
        }

        public Account(string username, string access_token, string refresh_token, string naslink, string sort_type, string systemInfoStr)
        {
            UserName = username;
            AccessToken = access_token;
            UserNasLink = naslink;
            ResfreshToken = refresh_token;
            SortType = sort_type;
            SystemInfoStr = systemInfoStr;
        }
    }
}