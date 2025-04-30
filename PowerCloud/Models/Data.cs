namespace PowerCloud.Models
{
    public class FileItem
    {
        public string FileName { get; set; }
        public string FileFormat { get; set; }
        public string FileLastModifiedDate { get; set; }
        public string FileSize { get; set; }
    }

    public class FolderItem
    {
        public string FolderName { get; set; }
    }
    
    public class LoginUserItem
    {
        public static int Count { get; internal set; }
        public string UserName { get; set; }
        public string UserNasLink { get; set; }
    }
    
    public class NASItem
    {
        public string NasIp { get; set; }
        public string NasName { get; set; }
    }    

    public class BackupItem
    {
        public string ImageUrl { get; set; }
    }
}