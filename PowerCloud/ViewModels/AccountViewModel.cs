using Newtonsoft.Json;

using PowerCloud.Models;

namespace PowerCloud.ViewModels
{

    public class AccountViewModel : BindViewModel
    {

        private string pUserName;
        private string pUserNasLink;
        private string pUserSecret;
        //private string pCurrentPath;
        private string pAccessToken;
        private string pRefreshToken;

        public AccountViewModel() : this(new Account())
        {
        }

        public AccountViewModel(Account copy)
        {
            RefreshToken = copy.ResfreshToken;
            AccessToken = copy.AccessToken;
            UserNasLink = copy.UserNasLink;
            UserName = copy.UserName;
            SystemInfoStr = copy.SystemInfoStr;
            SortType = copy.SortType;
            LoginedStr = "Check it";
        }

        public string UserName
        {
            get { return pUserName; }
            set { SetPropertyValue(ref pUserName, value); }
        }

        public string UserNasLink { get { return pUserNasLink; } set { SetPropertyValue(ref pUserNasLink, value); } }

        public string UserSecret
        {
            get { return pUserSecret; }
            set { SetPropertyValue(ref pUserSecret, value); }
        }

        public string AccessToken
        {
            get { return pAccessToken; }
            set
            {
                SetPropertyValue(ref pAccessToken, value);
                LoginedStr = "Ckeck it";
            }

        }

        public string RefreshToken
        {
            get { return pRefreshToken; }
            set { SetPropertyValue(ref pRefreshToken, value); }

        }

        //public string CurrentPath
        //{
        //    get { return pCurrentPath; }
        //    set { SetPropertyValue(ref pCurrentPath, value); }
        //}

        string systemInfoStr;
        public string SystemInfoStr
        {
            get { return systemInfoStr; }
            set
            {
                SetPropertyValue(ref systemInfoStr, value);

                if (SystemInfoStr.StartsWith("{") && SystemInfoStr.EndsWith("}"))
                    try
                    {
                        SystemInfo = JsonConvert.DeserializeObject<NE201SystemInfo>(SystemInfoStr);
                    }
                    catch
                    {
                        SystemInfo = new NE201SystemInfo();
                    }
                else
                {
                    SystemInfo = new NE201SystemInfo();

                }
            }
        }
        
        NE201SystemInfo systemInfo;
        public NE201SystemInfo SystemInfo { get { return systemInfo; } private set { SetPropertyValue(ref systemInfo, value); } }

        string pSortType;
        public string SortType
        {
            get { return pSortType; }
            set { SetPropertyValue(ref pSortType, value); }
        }

        string pLoginedStr;
        public string LoginedStr
        {
            get 
            {
                return pLoginedStr;
            }
            set 
            {
                bool result = true;
                if (string.IsNullOrEmpty(AccessToken) || AccessToken == "unknown")
                    result = false;
                string s = " ";
                if (result)
                    s = "\ue92b";
                else
                    s = " ";

                SetPropertyValue(ref pLoginedStr, s);
            }
        }
    }



    #region SystemInfo
    public class NE201SystemInfo
    {
        public string Model { get; set; }
        public string SN { get; set; }
        public string HostName { get; set; }
        public string Version { get; set; }
        public string DDNS { get; set; }
        public string OS { get; set; }
        public DateTime LoginAt { get; set; }
    }
    #endregion

    #region FileDeleteBody and FileManageResponse and NasResult
    public class FileDeleteBody
    {
        //{"SourcePath":null,"DestPath":null,"NewName":null,"items":["A.txt","B.txt","C.txt"],"UploadKey":null,"thumbSize":44,"FileSize":0}
        public string SourcePath { get; set; }
        //public string DestPath { get; set; }
        //public string NewName { get; set; }
        public string[] Items { get; set; }
        //public string UploadKey { get; set; }
        //private int tnail = 44;
        //public int thumbSize { get { return tnail; } set { tnail = value; } }
        //public double FileSize { get; set; }
    }


    public class FileManageResponse3
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public bool Result { get; set; }
    }


    public class FileManageResponse2
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public bool Result { get; set; }
        public string ErrorMessage { get; set; }
    }


    public class FileManageResponse
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public NASResult Result { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class NASResult
    {
        public int success { get; set; }
        public int error { get; set; }
    }
    #endregion
}