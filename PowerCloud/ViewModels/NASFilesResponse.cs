namespace PowerCloud.ViewModels
{
    public class NASFileResponse
    {
        //private string pVersion;
        //private int pStatusCode;
        //private List<NASFile> pFiles;
        //private int pTotals;
        //private string pErrorMessage;
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public NASFilesResponseInfo Result { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class NASFilesResponseInfo
    {
        public List<NASFileViewModel> Files { get; set; }
        public int Totals { get; set; }
    }
}
