using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using PowerCloud.ViewModels;

using System.Collections.ObjectModel;
using System.Web;
using System.Windows.Input;

namespace PowerCloud.Views.FileManagement;

public partial class View : ContentPage
{
    public View(MainNasFileViewModel prnMvm)
    {
        InitializeComponent();

        mvm = prnMvm;
        int nasIndex = mvm.NASFiles.IndexOf(mvm.FileSelected);
        
        x1 = new ObservableCollection<FileReviewViewModel>();

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //ImageList.IsVisible = false;
        //ActIndicator.IsRunning = true;
        int nasIndex = mvm.NASFiles.IndexOf(mvm.FileSelected);

        int n = mvm.CurrentMaxPage;
        await mvm.readFinalPage();
        while (true)
        {
            await mvm.readFileList(mvm.PrevPath, mvm.CurrentMaxPage + 1);

            if (n == mvm.CurrentMaxPage)
                break;
            n = mvm.CurrentMaxPage;
        }

        n = nasIndex;
        while (n < mvm.NASFiles.Count)
        {
            FileReviewViewModel? xItem = null;
            bool isVideoType = mvm.NASFiles[n].MimeType.StartsWith("video", true, null);

            if (isVideoType || mvm.NASFiles[n].MimeType.StartsWith("image"))
            {
                xItem = new FileReviewViewModel(mvm.NASFiles[n], n);
                xItem.ImageListIndex = x1.Count;
                x1.Add(xItem);
                if (isVideoType)
                    x1[xItem.ImageListIndex].IsVideo = true;
                else
                    x1[xItem.ImageListIndex].IsImage = true;

            }
            n++;
        }
        n = 0;
        while (n < nasIndex)
        {
            FileReviewViewModel? xItem = null;
            bool isVideoType = mvm.NASFiles[n].MimeType.StartsWith("video", true, null);

            if (isVideoType || mvm.NASFiles[n].MimeType.StartsWith("image"))
            {
                xItem = new FileReviewViewModel(mvm.NASFiles[n], n);
                xItem.ImageListIndex = x1.Count;
                x1.Add(xItem);
                if (isVideoType)
                    x1[xItem.ImageListIndex].IsVideo = true;
                else
                    x1[xItem.ImageListIndex].IsImage = true;

            }
            n++;
        }

        int currentIndex = 0;
        ImageList.ItemsSource = x1;

        if (x1.Count > 0)
        {
            ImageList.CurrentItem = x1[currentIndex];
            await ReadImage(x1[currentIndex]);
        }

        ActIndicator.IsRunning = false;
        ImageList.IsVisible = true;
    }

    // Demo code for calling pattern
    //
    //Routing.RegisterRoute("FileManagement_View", typeof(FileManagement_View));
    //await Shell.Current.GoToAsync($"FileManagement_View?ite2path=public");
    //
    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        if (query.Count > 0)
        {
            string name = HttpUtility.UrlDecode(query["ite2path"]);
        }
        else
            Console.WriteLine("..No parameter..");
    }

    protected override bool OnBackButtonPressed()
    {
        if (Navigation.NavigationStack.Count > 0)
        {
            //Navigation.PopAllPopupAsync();
            return true;
        }
        else
            return base.OnBackButtonPressed();
    }




    ObservableCollection<FileReviewViewModel> xxx;
    public ObservableCollection<FileReviewViewModel> x1 { get { return xxx; } set { xxx = value; } }

    public MainNasFileViewModel mvm;

    const int RefreshDuration = 2;
    private bool isRefreshing;
    public bool IsRefreshing
    {
        get { return isRefreshing; }
        set
        {
            isRefreshing = value;
            OnPropertyChanged("IsRefreshing");
        }
    }

    public ICommand RefreshCommand => new Command(async () => await RefreshDataAsync());
    public async Task RefreshDataAsync()
    {
        IsRefreshing = true;
        await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
        IsRefreshing = false;
    }


    //////按鈕事件
    //////popup_131_刪除(圖片瀏覽) Btn_Popup_FileManViewDelete
    ////private async void Btn_Popup_FileManViewDelete(object sender, EventArgs e)
    ////{
    ////    await Navigation.PushPopupAsync(new FileManagement_Popup_ViewDelete());
    ////    //await Navigation.PushModalAsync(new FileManagement_Popup_ViewDelete());
    ////}


    bool InItemChanged = false;
    object locker = new object();
    private async void ImageList_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        lock (locker)
        {
            if (InItemChanged)
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return;
            }
            InItemChanged = true;
        }

        ImageList.IsVisible = false;
        ActIndicator.IsRunning = true;
        if (e.PreviousItem != null)
            ((FileReviewViewModel)e.PreviousItem).ImageSrc = null;

        FileReviewViewModel item = (FileReviewViewModel)e.CurrentItem;
        //shellTitleView.Text = item.FileOnNas.Name;

        await ReadImage(item);

        ActIndicator.IsRunning = false;
        ImageList.IsVisible = true;
        TextBoard.BindingContext = item.FileOnNas;

        lock (locker)
        {
            InItemChanged = false;
        }
    }


    private async Task<bool> ReadImage(FileReviewViewModel file)
    {
        bool done = true;
        //string fExt = Path.GetExtension(file.FileOnNas.Name);
        //string localFile = Path.Combine(FileSystem.CacheDirectory, $"Ite2TmpImg{fExt}");
        string localFile = Path.Combine(FileSystem.CacheDirectory, file.FileOnNas.Name);

        if (!File.Exists(localFile))
        {
            await mvm.fmr.NE201Download(file.FileOnNas, localFile);

            //int buffLen = 4096 * 1024;
            //byte[] buffer = new byte[buffLen];
            //int byteRead = buffLen;
            //while (byteRead > 0)
            //{
            //    byteRead = await source.ReadAsync(buffer, 0, buffLen);
            //    if (byteRead > 0)
            //    {
            //        target.Write(buffer, 0, byteRead);
            //    }
            //}

            //return true;
        }
        if (file.IsImage)
            file.ImageSrc = ImageSource.FromFile(localFile);
        else
        {
            file.VideoSrc = MediaSource.FromFile(localFile);
        }

            //Image img = new Image { Source = ImageSource.FromFile(localFile) };
            //img.
            return done;
    }

    private void Btn_Popup_FileManViewDelete(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_ViewDelete();
        //popup-end
        //popup.VerticalOptions = LayoutAlignment.End;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        popup.VerticalOptions = LayoutOptions.Center;
        popup.HorizontalOptions = LayoutOptions.Fill;

        AppShell.Current.ShowPopup(popup);
    }
}







#region Help Class
public class FileReviewViewModel : BindViewModel
{
    public FileReviewViewModel(NASFileViewModel x, int nasFilesIndex)
    {
        x2 = x;
        NASFileIndex = nasFilesIndex;

        if (x.Name.EndsWith(".gif"))
        {
            IsGif = true;
        }
        else
        {
            if (x.MimeType.StartsWith("video", true, null))
                isVideo = true;
            else if (x.MimeType.StartsWith("image", true, null))
                isImage = true;
            else if (x.MimeType.StartsWith("audio", true, null))
                isAudio = true;
        }
    }

    NASFileViewModel x2;
    public int NASFileIndex { get; set; }
    public int ImageListIndex { get; set; }

    public NASFileViewModel FileOnNas { get { return x2; } set { SetPropertyValue(ref x2, value); } }

    public string prvImageSrcPath = string.Empty;

    ImageSource? src = null;
    public ImageSource? ImageSrc
    {
        get { return src; }
        set { SetPropertyValue(ref src, value); }
    }

    MediaSource? videoSrc = null;
    public MediaSource? VideoSrc
    {
        get { return videoSrc; }
        set { SetPropertyValue(ref videoSrc, value); }
    }

    bool isgif = false;
    public bool IsGif
    {
        get { return isgif; }
        internal set { SetPropertyValue(ref isgif, value); }
    }

    bool isImage = false;
    public bool IsImage
    {
        get { return isImage; }
        internal set { SetPropertyValue(ref isImage, value); }
    }

    bool isVideo = false;
    public bool IsVideo
    {
        get { return isVideo; }
        internal set { SetPropertyValue(ref isVideo, value); }
    }

    bool isAudio = false;
    public bool IsAudio
    {
        get { return isAudio; }
        internal set { SetPropertyValue(ref isAudio, value); }
    }
    #endregion
}
