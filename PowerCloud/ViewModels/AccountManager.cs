using System.Collections.ObjectModel;

namespace PowerCloud.ViewModels
{
    public class AccountManager
    {
        private IAccountFiler2 loader;
        //private ITextToSpeech tts;
        private IUserTakePicture tts;
        private IPdfRender pdfRender;
        private IIte2DeviceInfo2 devInfo;

        public static AccountManager Instance { get; private set; }

        public IList<AccountViewModel> Accounts { get; set; }

        public AccountManager(IAccountFiler2 loader, /*ITextToSpeech*/IUserTakePicture tts, IPdfRender pdfR, IIte2DeviceInfo2 prvdev)
        {

            if (Instance != null)
            {
                throw new Exception("Can only create a single UserManager.");
            }
            Instance = this;

            this.loader = loader;
            this.tts = tts;
            pdfRender = pdfR;
            devInfo = prvdev;
            Accounts = new ObservableCollection<AccountViewModel>(loader.Load());
        }

        public void Save(IList<AccountViewModel> l1, IList<AccountViewModel> l2)
        {
            List<AccountViewModel> k = new List<AccountViewModel>();
            if (l1 != null)
            {
                foreach (AccountViewModel item in l1)
                    k.Add(item);
            }
            if (l2 != null)
            {
                foreach (AccountViewModel item in l2)
                    k.Add(item);
            }
            //loader.Save(Accounts);
            loader.Save(k);
        }

        //public void SayQuote(GreatQuoteViewModel quote) {
        //    if (quote == null)
        //        throw new ArgumentNullException("No quote set");

        //    if (tts != null){
        //        var text = quote.QuoteText;

        //        if (!string.IsNullOrWhiteSpace(quote.Author))
        //            text += $" by {quote.Author}";

        //        tts.Speak(text);
        //    }
        //}

        public void TakePicture(AccountViewModel user)
        {
            if (user == null)
                throw new ArgumentNullException("No user set");

            if (tts != null)
            {
                var text = user.UserNasLink;

                if (!string.IsNullOrWhiteSpace(user.UserNasLink))
                    text += $" by {user.UserName}";

                //tts.Speak(text);
                tts.TakePicture(text);
            }
        }

        public void PDFRender(string something, MainNasFileViewModel fvm)
        {
            if (string.IsNullOrEmpty(something))
                throw new ArgumentNullException("No file name");

            if (pdfRender != null)
                pdfRender.Render(something, fvm);
        }
        public async Task<string> GetExternalPath(AccountViewModel mvm)
        {
            if (devInfo == null)
                throw new ArgumentNullException("Get External-Path Name failure");

            return await devInfo.GetExternalPath(mvm);
        }

        public async Task DownloadToDevice(MainNasFileViewModel mvm)
        {
            if (devInfo == null)
                throw new ArgumentNullException("Download failure");

            await devInfo.DownloadToDevice(mvm);
        }

        public async Task<Stream> GetDownloadStream(NASFileViewModel nasFile)
        {
            if (devInfo == null)
                throw new ArgumentNullException("Download failure");

            return await devInfo.GetDownloadStream(nasFile);
        }

        public void GetPhotoLibrary(MainNasFileViewModel prmMvm)
        {
            if (devInfo == null)
                throw new ArgumentNullException("Get photo-library failure");

            devInfo.GetPhotoLibrary(prmMvm);
        }

        public List<AlbumItem> GetNativeFileIDs()
        {
            return devInfo.GetNativeFileIDs();
        }

        public Task<bool> UploadMediaFile(AlbumItem item)
        {
            return devInfo.UploadMediaFile(item);
        }

        public Task<byte[]> GetThumbnail(AlbumItem item)
        {
            return devInfo.GetThumbnail(item);
        }

        public Task<bool> CopyToLocal(AlbumItem sourece, string target)
        {
            return devInfo.CopyToLocal(sourece, target);
        }

        public string SelectedNasAddress 
        {
            get { return devInfo.SelectedNasAddress; }
            set { devInfo.SelectedNasAddress = value; }
        }

        public string SelectedRefreshToken 
        { 
            get { return devInfo.SelectedRefreshToken; } 
            set { devInfo.SelectedRefreshToken = value; } 
        }

        public string SelectedAccessToeken 
        { 
            get { return devInfo.SelectedAccessToken; } 
            set { devInfo.SelectedAccessToken = value; } 
        }
    }
}