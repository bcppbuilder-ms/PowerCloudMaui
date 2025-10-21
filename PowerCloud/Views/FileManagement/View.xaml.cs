using CommunityToolkit.Maui.Views;
using PowerCloud.ViewModels;
using System.Collections.ObjectModel;
using System.Web;
using System.Windows.Input;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

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
            if (mvm.NASFiles[n].MimeType.StartsWith("image"))
            {
                FileReviewViewModel xItem = new FileReviewViewModel(mvm.NASFiles[n], n);
                xItem.ImageListIndex = x1.Count;
                x1.Add(xItem);
            }
            n++;
        }
        n = 0;
        while (n < nasIndex)
        {
            if (mvm.NASFiles[n].MimeType.StartsWith("image"))
            {
                FileReviewViewModel xItem = new FileReviewViewModel(mvm.NASFiles[n], n);
                xItem.ImageListIndex = x1.Count;
                x1.Add(xItem);
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


    //////���s�ƥ�
    //////popup_131_�R��(�Ϥ��s��) Btn_Popup_FileManViewDelete
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
        file.ImageSrc = ImageSource.FromFile(localFile);

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
        popup.VerticalOptions = LayoutAlignment.Center;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
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
            IsGif = false;
        }
    }

    NASFileViewModel x2;
    public int NASFileIndex { get; set; }
    public int ImageListIndex { get; set; }

    public NASFileViewModel FileOnNas { get { return x2; } set { SetPropertyValue(ref x2, value); } }

    public string prvImageSrcPath;

    ImageSource src;
    public ImageSource ImageSrc
    {
        get { return src; }
        set { SetPropertyValue(ref src, value); }
    }

    bool isgif;
    public bool IsGif
    {
        get { return isgif; }
        private set { SetPropertyValue(ref isgif, value); }
    }
    #endregion
}
