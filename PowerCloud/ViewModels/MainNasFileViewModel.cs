using Newtonsoft.Json;
using PowerCloud.NasHttp;

using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Windows.Input;

namespace PowerCloud.ViewModels
{
    public class MainNasFileViewModel : BindViewModel, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            // Only a single query parameter is passed, which needs URL decoding.
            //collectionView.RemainingItemsThreshold = 6;
            string pathname = string.Empty;
            if (query.Count > 0 && !string.IsNullOrEmpty(query["ite2path"]))
            {
                pathname = HttpUtility.UrlDecode(query["ite2path"]);
            }
            else
                return;
            LoadPath(pathname);
        }

        async void LoadPath(string path)
        {
            CurrentMaxPage = 0;
            PrevPath = path;
            NASFiles = new ObservableCollection<NASFileViewModel>();
            //await ListView_RefreshFolder();
            await GetNextPageOfData();
            PageCollectionView.ItemsSource = NASFiles;
        }

        public async Task<string> GetNewFileName(string fullName)
        {
            string fileName = Path.GetFileName(fullName);
            string fExt = Path.GetExtension(fileName);
            string fBaseName = fileName.Substring(0, fileName.Length - fExt.Length);
            string pathName = Path.GetDirectoryName(fullName);
            if (pathName == fullName)
                pathName = "";
            int seq = 0;
            while (await fmr.NE201IsFileExist(fullName))
            {
                seq++;
                fullName = Path.Combine(pathName, fBaseName) + $"_{seq}{fExt}";
            }
            return fullName;
        }

        public async Task<bool> Copy(NASFileViewModel file, bool reursive)
        {
            await copySingleFile(Path.Combine(file.PathName, file.Name));

            return true;
        }

        private async Task copySingleFile(string fullFile)
        {
            string newFileName = await GetNewFileName(fullFile);
            await fmr.NE201CopyFile(fullFile, newFileName);
        }

        int prvPageSize = 150;
        public int PageSize { get { return prvPageSize; } /*set { SetPropertyValue(ref prvPageSize, value); }*/ }

        int prvCurrentMaxPage;
        public int CurrentMaxPage { get { return prvCurrentMaxPage; } set { SetPropertyValue(ref prvCurrentMaxPage, value); } }

        string prvPrevPath;
        public string PrevPath
        {
            get { return prvPrevPath; }
            set { SetPropertyValue(ref prvPrevPath, value); }
        }


        //ObservableCollection<NASFileViewModel> nasfiles;
        public ObservableCollection<NASFileViewModel> NASFiles { get; set; } = new ObservableCollection<NASFileViewModel>(); 

        NASFileViewModel fileselected;
        public NASFileViewModel FileSelected { get { return fileselected; } set { SetPropertyValue(ref fileselected, value); } }

        string tmpimagepath;
        public string TempImagePath { get { return tmpimagepath; } set { SetPropertyValue(ref tmpimagepath, value); } }


        int filesCount;
        public int FilesCount { get { return filesCount; } set { SetPropertyValue(ref filesCount, NASFiles.Count); } }

        private string errMsg;
        public string ErrorMessage { get { return errMsg; } set { SetPropertyValue(ref errMsg, value); } }

        public CollectionView PageCollectionView { get; set; }



        public List<NASFileViewModel> PayLoad = new List<NASFileViewModel>();
        public int AddFiles(bool clearAll = true)
        {
            if (clearAll)
                PayLoad.Clear();
            int n = 0;
            foreach (NASFileViewModel item in NASFiles)
            {
                if (item.Selected)
                {
                    n++;
                    //item.Selected = false;
                    NASFileViewModel newItem = new NASFileViewModel();
                    replaceinfo(newItem, item, true);
                    PayLoad.Add(newItem);
                }
            }
            if (n == 0)
            {
                if (fileselected != null)
                {
                    NASFileViewModel newItem = new NASFileViewModel();
                    replaceinfo(newItem, fileselected, true);
                    PayLoad.Add(newItem);
                    //FileSelected = null;
                }
            }

            return PayLoad.Count;
        }



        public int thumbSize = 354;


        private bool useThumbNail = false;
        public bool UseThumbNail
        {
            get { return useThumbNail; }
            set
            {
                bool oldvalue = useThumbNail;
                SetPropertyValue(ref useThumbNail, value);
            }
        }



        const int RefreshDuration = 2;
        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                //isRefreshing = value;
                SetPropertyValue(ref isRefreshing, value);
            }
        }
        public ICommand RefreshCommand => new Command(async () => await RefreshDataAsync());
        public async Task RefreshDataAsync()
        {
            //IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            IsRefreshing = false;
        }

        public ICommand SetSelectedCommand => new Command<NASFileViewModel>(SetSelected);
        private void SetSelected(NASFileViewModel file)
        {
            FileSelected = file;
        }

        public ICommand LoadMoreDataCommand => new Command(async () => await GetNextPageOfData());



        bool inuse = false;
        object kObj = new object();

        public async Task<int> GetNextPageOfData()
        {
            if (string.IsNullOrEmpty(PrevPath))
            {
                return 0;
                //return Task.Run(() => 0);
            }

            lock (kObj)
            {
                if (inuse)
                {
                    //Console.WriteLine("Blocked GetNextPageOfData: {0}, {1}, {2}, {3}, {4}", 
                    //    firstVisibleItem, centerItem, lastVisibleItem, PrevPath, NASFiles.Count);
                    return -1;
                    //return Task.Run(() => -1);
                }
                inuse = true;
            }

            //Console.WriteLine("Go GetNextPageOfData: {0}, {1}, {2}, {3}, {4}",
            //    firstVisibleItem, centerItem, lastVisibleItem, PrevPath, NASFiles.Count);
            try
            {
                if (NASFiles.Count == 0)
                {
                    FilesCount = 0;
                    CurrentMaxPage = 0;
                    await readFileList(PrevPath, CurrentMaxPage + 1);
                    //readFileList(PrevPath, CurrentMaxPage + 1);
                }
                else
                {
                    //if (NASFiles.Count - lastVisibleItem <= PageCollectionView.RemainingItemsThreshold)
                    //{
                    if (NASFiles.Count == PageSize * CurrentMaxPage)
                        await readFinalPage();
                    //}
                }
            }
            catch (Exception allErr)
            {
                string s = allErr.ToString();
                if (allErr.Data.Count > 0)
                {
                    s += "\r\n\r\n";
                    foreach (DictionaryEntry x in allErr.Data)
                    {
                        s += x.Key.ToString() + x.Value.ToString();
                    }
                    //Console.WriteLine(s);
                }
            }

            FilesCount = NASFiles.Count;
            //Console.WriteLine("Finished GetNextPageOfData: {0}, {1}, {2}, {3}, {4}",
            //    firstVisibleItem, centerItem, lastVisibleItem, PrevPath, NASFiles.Count);

            lock (kObj)
            {
                inuse = false;
            }

            return NASFiles.Count;
            //return Task.Run(() => NASFiles.Count);
        }


        public async Task<bool> readFinalPage()
        {
            int remain = CurrentMaxPage * PageSize - NASFiles.Count;
            if (remain <= 0)    // < 0 means somebody add item to it but we don't know.
            {
                return await readFileList(PrevPath, CurrentMaxPage + 1);
            }
            else
            {
                int n = PageSize - remain;
                
                await readFileList(PrevPath, CurrentMaxPage, n);
                
                if (NASFiles.Count == CurrentMaxPage * PageSize)
                {
                    await readFileList(PrevPath, CurrentMaxPage + 1);
                }
                return true;
            }
        }

        private void RemoveExtraFiles(int newCount)
        {
            if (newCount >= NASFiles.Count)
                return;

            while (newCount < NASFiles.Count)
            {
                int n = NASFiles.Count;
                NASFiles.RemoveAt(n - 1);
            }

            int pageCount = NASFiles.Count / PageSize;
            if (NASFiles.Count > pageCount * PageSize)
                pageCount++;
            CurrentMaxPage = pageCount;
        }

        HttpClientHandler NASHttpHandler;
        HttpClient VMHttpClient;

        //HttpClientHandler NAS2HttpHandler;
        //HttpClient VM2HttpClient;
        public void ResetHttpClient()
        {
            VMHttpClient = null;
            NASHttpHandler = null;
        }


        NE201FileManager myFmr;
        public NE201FileManager fmr 
        {
            get 
            { 
                if (myFmr == null) 
                    myFmr = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected); 
                return myFmr; 
            }
        }




        byte[] buffer = new byte[0];
        long buffLen = 1024 * 1024;

        public async Task<bool> DownloadFile(NASFileViewModel file)
        {
            long allSize = file.Size;
            using (Stream sourceStream = await fmr.NE201DownloadStream(file))
            {
                using (Stream deviceStream = await App.PC2ViewModel.accountManager.GetDownloadStream(file))
                {
                    //Stream deviceStream = App.PC2ViewModel.accountManager.GetDownloadStream(file.FileOnNas);
                    if (sourceStream != null && deviceStream != null)
                    {
                        //await sourceStream.CopyToAsync(deviceStream);

                        var buffer = new byte[buffLen];
                        int read;
                        long readCount = 0;
                        while (true)
                        {
                            read = await sourceStream.ReadAsync(buffer, 0, (int)buffLen);
                            if (read <= 0)
                                break;
                            readCount += read;
                            await deviceStream.WriteAsync(buffer, 0, read);
                        }
                        file.Selected = false;
                    }
                }
            }

            return true;
        }

        public async Task<Stream> GetDownloadStream(NASFileViewModel file)
        {
            return await fmr.NE201DownloadStream(file);
        }









        internal async Task<bool> ListView_GotoSubFolder(/*CollectionView NASFileList*/)
        {
            //CollectionView NASFileList = PageCollectionView;
            if (FileSelected != null)   //.SelectedItem != null)
            {
                NASFileViewModel x = FileSelected;  // (NASFileViewModel)NASFileList.SelectedItem;
                if (x.MimeType == "folder")
                {
                    bool stopDoing = false;
                    lock (kObj)
                    {
                        if (inuse)
                            stopDoing = true;
                        else
                            inuse = true;
                    }
                    if (stopDoing)
                    {
                        //throw new Exception("Recursive doing !");
                        ErrorMessage = "Concurrently doing with page-refreshing!";
                        return false;
                    }
                    string path = x.PathName;
                    if (path != PrevPath)
                    {
                        //throw new Exception("PrvPath -- Error");
                        PrevPath = path;
                    }
                    scrollStack.Push(new ScrolledInfo()
                    {
                        vOffset = vvOffset,
                        vDelta = vvDelta,
                        highest = firstVisibleItem,
                        path = PrevPath,
                        maxPage = CurrentMaxPage,
                        selected = FileSelected
                    });
                    PrevPath = Path.Combine(path, x.Name);
                    CurrentMaxPage = 0;
                    await readFileList(PrevPath, 1);
                    lock (kObj)
                    {
                        inuse = false;
                    }
                    //readFileList(PrevPath, 1);
                    ////NASFileList.SelectedItem = null;
                    FileSelected = null;
                }
                else //if (x.MimeType == "image/jpeg")
                {
                    //TempImagePath = PrevPath + "/" + x.Name;
                    ////await this.Navigation.PushModalAsync(new NavigationPage(new NASImagePage()));
                    //await this.Navigation.PushAsync(new NASImagePage());
                }
            }
            return true;
            //return Task.Run(() => true);
        }

        internal async Task<bool> GotoParentFolder(/*CollectionView NASFileList*/)
        {
            string upper = Path.GetDirectoryName(PrevPath);
            PrevPath = upper;
            NASFiles.Clear();
            CurrentMaxPage = 0;
            if (string.IsNullOrEmpty(upper))
            {
                NASFiles.Add(new NASFileViewModel() { PathName = "", MimeType = "folder", Name = "home", ImageSrc = null});
                NASFiles.Add(new NASFileViewModel() { PathName = "", MimeType = "folder", Name = "public", ImageSrc = null });
                NASFiles.Add(new NASFileViewModel() { PathName = "", MimeType = "folder", Name = "group", ImageSrc = null });
            }
            else
            {
                await readFileList(PrevPath, CurrentMaxPage + 1);
            }
            return true;
            //return Task.Run<bool>(delegate { return true; });
        }

        internal async Task<bool> ListView_GotoParentFolder(/*CollectionView NASFileList*/)
        {
            if (scrollStack.Count > 0)
            {
                bool stopDoing = false;
                lock (kObj)
                {
                    if (inuse)
                        stopDoing = true;
                    else
                        inuse = true;
                }
                if (stopDoing)
                {
                    //throw new Exception("Recursive doing !");
                    ErrorMessage = "Concurrently doing with page-refreshing!";
                    return false;
                }

                ScrolledInfo scr = scrollStack.Pop();
                vvOffset = scr.vOffset;
                vvDelta = scr.vDelta;
                firstVisibleItem = scr.highest;
                CurrentMaxPage = scr.maxPage;
                PrevPath = scr.path;
                CurrentMaxPage = 0;

                for (int i = 0; i < scr.maxPage; i++)
                {
                    //readFileList(PrevPath, i + 1);
                    await readFileList(PrevPath, i + 1);
                }

                PageCollectionView.ScrollTo(scr.highest, -1, ScrollToPosition.Start);
                FileSelected = scr.selected;
                lock (kObj)
                {
                    inuse = false;
                }
            }

            return true;
            //return Task.Run<bool>(delegate { return true; });
        }

        public void ScrollToAnchor(ScrollToPosition x = ScrollToPosition.Start)
        {
            if (x is ScrollToPosition.Start)
                PageCollectionView.ScrollTo(firstVisibleItem, -1, ScrollToPosition.Start);
            else if (x is ScrollToPosition.Center)
                PageCollectionView.ScrollTo(centerItem, -1, ScrollToPosition.Center);
            else
                PageCollectionView.ScrollTo(lastVisibleItem, -1, ScrollToPosition.End);
        }

        public async Task ResetImageSrc(bool newUsingThumb)
        { 
            foreach (NASFileViewModel item in NASFiles)
            {
                if (newUsingThumb && item.MimeType.StartsWith("image") && item.thumbNail.Length == 0)
                {
                    item.thumbNail = await fmr.NE201ImageThumbnail(Path.Combine(item.PathName, item.Name), thumbSize);
                    //item.thumbNail = fmr.NE201ImageThumbnail(Path.Combine(item.PathName, item.Name), thumbSize);
                }
                item.UsingThumb = newUsingThumb;
            }
        }

        internal void ListView_Scrolled(ItemsViewScrolledEventArgs e)
        {
            firstVisibleItem = e.FirstVisibleItemIndex;
            centerItem = e.CenterItemIndex;
            lastVisibleItem = e.LastVisibleItemIndex;
            hhOffset = e.HorizontalOffset;
            hhDelta = e.HorizontalDelta;
            vvDelta = e.VerticalDelta;
            vvOffset = e.VerticalOffset;
        }

        Stack<ScrolledInfo> scrollStack = new Stack<ScrolledInfo>();

        int firstVisibleItem = 0;
        int centerItem = 0;
        int lastVisibleItem = 0;
        double hhOffset;
        double hhDelta;
        double vvOffset;
        double vvDelta;
        public int ScrollHighest { get { return firstVisibleItem; } set { firstVisibleItem = value; } }




        private void replaceinfo(NASFileViewModel old, NASFileViewModel fi, bool newThumbNail = false)
        {
            if (old.Name != fi.Name)
                old.Name = fi.Name;
            if (old.PathName != fi.PathName)
                old.PathName = fi.PathName;
            if (old.Size != fi.Size)
                old.Size = fi.Size;
            if (old.MimeType != fi.MimeType)
                old.MimeType = fi.MimeType;
            if (old.CreationTime != fi.CreationTime)
                old.CreationTime = fi.CreationTime;
            if (old.LastWriteTime != fi.LastWriteTime)
                old.LastWriteTime = fi.LastWriteTime;
            if (old.FileAttr != fi.FileAttr)
                old.FileAttr = fi.FileAttr;
            if (old.FileAttrContent != fi.FileAttrContent)
                old.FileAttrContent = fi.FileAttrContent;
            if (old.MediaType != fi.MediaType)
                old.MediaType = fi.MediaType;

            if (old.Selected != fi.Selected)
                old.Selected = fi.Selected;

            if (old.CanMultiSelect != fi.CanMultiSelect)
                old.CanMultiSelect = fi.CanMultiSelect;

            if (newThumbNail)
            {
                byte[] buff = new byte[fi.thumbNail.Length];
                if (buff.Length > 0)
                    Array.Copy(fi.thumbNail, buff, buff.Length);
                old.thumbNail = buff;
            }
            else
                old.thumbNail = fi.thumbNail;

            old.UsingThumb = useThumbNail;
        }



        #region HTTP IO
        public async Task<bool> readAllFileList(string sourcepath, int totCount)
        {
            int maxPage = CurrentMaxPage;
            int remain = NASFiles.Count; ;
            for (int i = 0; i < maxPage; i++)
            {
                if (remain < PageSize)
                {
                    await readFileList(PrevPath, i + 1, remain);
                }
                else
                {
                    await readFileList(PrevPath, i + 1, PageSize);
                }
                remain -= PageSize;
            }
            await readFinalPage();

            //PageCollectionView.ScrollTo(firstVisibleItem, -1, ScrollToPosition.Start);
            FilesCount = NASFiles.Count; 
            FileSelected = null;

            return true;
        }





        public async Task<bool> readFileList(string sourcepath, int pageNo, int oldPageLen = 0, ActivityIndicator actIndicator = null, [CallerMemberName] string propertyName = null)
        {
            int firstPageItemIndex = (pageNo - 1) * PageSize;
            if (firstPageItemIndex < 0)
                firstPageItemIndex = 0;
            ErrorMessage = string.Empty;
            Uri ne201uri;
            try
            {
                ne201uri = new Uri(string.Format("https://{0}", App.PC2ViewModel.UserSelected.UserNasLink) + "/FileManage/FileList");
            }
            catch
            {
                ErrorMessage = "Invalid URI";
                if (actIndicator != null)
                    actIndicator.IsRunning = false;
                return false;
            }

            int redoCount = 0;
        redo:
            if (VMHttpClient == null)
            {
                NASHttpHandler = new HttpClientHandler();
                NASHttpHandler.ServerCertificateCustomValidationCallback = NE201Login.ServerCertificateCustomValidation;
                VMHttpClient = new HttpClient(NASHttpHandler);
                //VMHttpClient.Timeout = TimeSpan.FromSeconds(15);
            }

            VMHttpClient.DefaultRequestHeaders.Clear();
            VMHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", App.PC2ViewModel.UserSelected.AccessToken);
            string postBody = $"SourcePath={sourcepath}&page={pageNo}&pagesize={PageSize}"; //serachstr=*&
            HttpResponseMessage response = null;
            try
            {
                response = await VMHttpClient.PostAsync(ne201uri, new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType));
            }
            catch (Exception allErr)
            {
                ErrorMessage = allErr.Message;
            }

            //HttpResponseMessage response = VMHttpClient.PostAsync(ne201uri, new StringContent(postBody, Encoding.UTF8, NE201Login.mediaType)).GetAwaiter().GetResult();
            string result = string.Empty;
            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                 ErrorMessage = "wait deserializing ....";
                result = await response.Content.ReadAsStringAsync();
                //result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                NASFileResponse nasF = JsonConvert.DeserializeObject<NASFileResponse>(result);
                if (pageNo == 1 && oldPageLen == 0)
                {
                    ErrorMessage = "Initializing date";
                    NASFiles.Clear();
                    FileSelected = null;
                }
                if (nasF.Result.Files.Count > 0 /*&& nasF.Result.Files.Count > NASFiles.Count*/)
                {
                    ErrorMessage = "Add data to NASFiles";
                    int index = 0;
                    int httpRspCount = nasF.Result.Files.Count;
                    foreach (NASFileViewModel fi in nasF.Result.Files)
                    {
                        index++;
                        fi.PathName = sourcepath;
                        if (UseThumbNail)
                        {
                            if (fi.MimeType.StartsWith("image"))
                            {
                                byte[] buffer = new byte[0];
                                buffer = await fmr.NE201ImageThumbnail(Path.Combine(fi.PathName, fi.Name), thumbSize);
                                //buffer = fmr.NE201ImageThumbnail(Path.Combine(fi.PathName, fi.Name), thumbSize);
                                int n = buffer.Length;
                                if (n > 0)
                                {
                                    fi.thumbNail = new byte[n];
                                    Array.Copy(buffer, fi.thumbNail, n);
                                }
                            }
                        }
                        fi.UsingThumb = UseThumbNail;
                        if (oldPageLen == 0)
                        {
                            NASFiles.Add(fi);
                        }
                        else
                        {
                            int ind = firstPageItemIndex + index - 1;
                            if (index > oldPageLen || ind >= NASFiles.Count)
                            {
                                NASFiles.Add(fi);
                                //Console.WriteLine("Add data in replace-mode: {0}, {1}, {2}, {3}, {4}",
                                //    firstVisibleItem, centerItem, lastVisibleItem, PrevPath, NASFiles.Count);
                            }
                            else
                            {
                                if (NASFiles[ind].Name == fi.Name)
                                {
                                    fi.Selected = NASFiles[ind].Selected;
                                }
                                replaceinfo(NASFiles[ind], fi);
                            }
                        }
                    }
                    if (pageNo > CurrentMaxPage)
                        CurrentMaxPage = pageNo;

                    if (oldPageLen > 0 && httpRspCount < oldPageLen)
                    {
                        if (httpRspCount != index)
                            Console.WriteLine(".....................Item Count Error!.......................");
                        RemoveExtraFiles(firstPageItemIndex + httpRspCount);
                    }

                    ErrorMessage = "OK";
                }
                else
                {
                    ErrorMessage = $"No data for {sourcepath}, page: {pageNo}";
                    //Console.WriteLine(ErrorMessage);
                }

                if (actIndicator != null)
                    actIndicator.IsRunning = false;
                return true;
            }
            else
            {
                if (response == null)
                {
                    VMHttpClient = null;
                }
                else
                {
                    ErrorMessage = await response.Content.ReadAsStringAsync();
                    //ErrorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    bool loginSuccess = false;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ErrorMessage = "Refreshing AccessToken!";
                        loginSuccess = await NE201Login.RefreshLogin(App.PC2ViewModel.UserSelected);
                        //loginSuccess = NE201Login.RefreshLogin(App.PC2ViewModel.UserSelected).GetAwaiter().GetResult();
                        if (loginSuccess)
                        {
                            ErrorMessage = $"Redo readpage. Retried: {redoCount}.";
                        }
                    }
                    else
                    {
                        ErrorMessage = "Error Http StatusCode: " + response.StatusCode.ToString();
                        VMHttpClient = null;
                    }
                }

                //Console.WriteLine("Reading NASFiles failed. path: {0}, count: {1}, retry: {2}, {3}",
                //    PrevPath, NASFiles.Count, redoCount, ErrorMessage);
                redoCount++;

                if (redoCount > 5)
                {
                    ErrorMessage = "REDO Overflow; " + ErrorMessage;
                    //Console.WriteLine("Reading NASFiles failed. path: {0}, count: {1}, retry: {2}, {3}",
                    //    PrevPath, NASFiles.Count, redoCount, ErrorMessage);
                    if (actIndicator != null)
                        actIndicator.IsRunning = false;
                    return false;
                }
                goto redo;
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}