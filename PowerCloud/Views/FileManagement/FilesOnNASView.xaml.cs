using PowerCloud.ViewModels;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views.FileManagement;

public partial class FilesOnNASView : ContentView
{
	public FilesOnNASView()
	{
		InitializeComponent();

        if (string.IsNullOrEmpty(InitPath))
            mvm = new MainNasFileViewModel();
        else
        {
            mvm = new MainNasFileViewModel()
            {
                CurrentMaxPage = 0,
                PrevPath = InitPath,
                NASFiles = new ObservableCollection<NASFileViewModel>(),
                PageCollectionView = NASFileList
            };
        }
        BindingContext = mvm;
        InformationBoard.BindingContext = this;
        GridBottom.BindingContext = this;

        //if (DeviceInfo.Platform == DevicePlatform.iOS)
        //    ThisRefreshView.HeightRequest = 600;
    }




    public static readonly BindableProperty InitPathProperty = BindableProperty.Create(
        nameof(InitPath), typeof(string), typeof(FilesOnNASView),
        defaultValue: string.Empty, defaultBindingMode: BindingMode.OneWay, propertyChanged: InitPathPropertyChanged);

    private static void InitPathPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        FilesOnNASView control = (FilesOnNASView)bindable;
        if (oldValue != null && oldValue.ToString() != newValue.ToString())
        {
            //((MainNasFileViewModel)control.BindingContext).PrevPath = newValue?.ToString();
        }
    }

    public string? InitPath
    {
        get { return base.GetValue(InitPathProperty)?.ToString(); }
        set
        {
            string? s = GetValue(InitPathProperty)?.ToString();
            //if (string.IsNullOrEmpty(s) || s != value)
            {
                base.SetValue(InitPathProperty, value);
                mvm = new MainNasFileViewModel()
                {
                    CurrentMaxPage = 0,
                    PrevPath = value,
                    NASFiles = new ObservableCollection<NASFileViewModel>(),
                    PageCollectionView = NASFileList
                };
                mvm.ResetHttpClient();
                BindingContext = mvm;
            }
        }
    }




    public static readonly BindableProperty IsMultiSelectProperty = BindableProperty.Create(
        nameof(IsMultiSelect), typeof(bool), typeof(FilesOnNASView),
        defaultValue: false, defaultBindingMode: BindingMode.OneWay, propertyChanged: IsMultiSelectPropertyChanged);

    private static void IsMultiSelectPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue)
        {
            //((FilesOnNASView)bindable).displayListOrView();
        }
    }

    public bool IsMultiSelect
    {
        get
        {
            object obj = GetValue(IsMultiSelectProperty);
            if (obj == null)
                return false;

            return (bool)obj;
        }
        set
        {
            SetValue(IsMultiSelectProperty, value);
            if (value)
            {
                cbIsMultiSelect.IsChecked = true;
            }
            else
            {
                cbIsMultiSelect.IsChecked = false;
            }
            HasParent = false;
        }
    }




    public static readonly BindableProperty HasParentProperty = BindableProperty.Create(
        nameof(HasParent), typeof(bool), typeof(FilesOnNASView),
        defaultValue: false, defaultBindingMode: BindingMode.OneWay, propertyChanged: HasParentPropertyChanged);

    private static void HasParentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue)
        {
            FilesOnNASView control = (FilesOnNASView)bindable;
            if (control.OnHasParentChanged != null)
            {
                control.OnHasParentChanged.Invoke(newValue, new EventArgs());
            }
        }
    }

    public bool HasParent
    {
        get
        {
            object obj = GetValue(HasParentProperty);
            if (obj == null)
                return false;

            return (bool)obj;
        }
        set
        {
            if (IsMultiSelect)
            {
                SetValue(HasParentProperty, false);
                return;
            }

            if (BindingContext == null)
                return;

            MainNasFileViewModel mvm = (MainNasFileViewModel)BindingContext;

            if (mvm.PrevPath.Contains(Path.DirectorySeparatorChar.ToString()))
                SetValue(HasParentProperty, true);
            else
                SetValue(HasParentProperty, false);
        }
    }




    //string xicon01 = "\ue979";
    //public string IconChar01 { get { return xicon01; } set { xicon01 = value; OnPropertyChanged("IconChar01"); } }



    MainNasFileViewModel mvm;

    public event EventHandler? OnHasParentChanged;



    //按鈕事件
    //View_141_子資料夾
    //async void Btn_View_FileManagement_SubFolder(object sender, System.EventArgs e)
    //{
    //    //Routing.RegisterRoute(nameof(Views.FileManagement_SubFolder), typeof(Views.FileManagement_SubFolder));
    //    //await Shell.Current.GoToAsync(nameof(Views.FileManagement_SubFolder));
    //}

    //View_118-1_多選-列表 Btn_View_FileManagement_MultiSelect_ListView

    //popup_112_新增
    private void Btn_Popup_FileManSort(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_Sort();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        Shell.Current.ShowPopup(popup);
    }


    private void Btn_Popup_FileManAdd(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_Add();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        Shell.Current.ShowPopup(popup);
    }



    bool doing = false;



    ItemsLayout? listLayout;
    DataTemplate? listTemplate;
    GridItemsLayout gridItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 };



    AccountViewModel currentUser = null;

    private void FileList_ListView_Public_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        mvm.ListView_Scrolled(e);
    }

    private async void NASFileList_RemainingItemsThresholdReached(object sender, EventArgs e)
    {
        ActIndicator.IsRunning = true;
        await mvm.GetNextPageOfData();
        ActIndicator.IsRunning = false;
    }

    private async void NASFileLists_Tapped(object sender, EventArgs e)
    {
        if (mvm.FileSelected == null)
            return;

        if (mvm.FileSelected.MimeType == "folder")
        {
            await AppShell.Current.ShowPopupAsync(new Popup_SelectFolder(mvm));
        }
        else
        {
            await AppShell.Current.ShowPopupAsync(new Popup_SelectFile(mvm));
        }
    }
    //private async void NASFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    if (((NASFileViewModel)NASFileList.SelectedItem).MimeType == "folder")
    //    {
    //        await Navigation.PushPopupAsync(new FileManagement_Popup_SelectFolder(mvm));
    //    }
    //    else
    //    {
    //        //Routing.RegisterRoute(nameof(FileManagement_View), typeof(FileManagement_View));
    //        //await Shell.Current.GoToAsync(nameof(FileManagement_View));
    //        if (mvm.FileSelected.MimeType.StartsWith("image"))
    //            await Navigation.PushAsync(new FileManagement_View(mvm));
    //        else
    //        {
    //            await Navigation.PushPopupAsync(new Popup_ShowMessage("Image File Only", "File mime type must be \"image\"", "Close"));
    //        }
    //    }
    //}

    private async void thumb_pic_Tapped(object sender, EventArgs e)
    {
        //App.PC2ViewModel.PicViewPath = mvm.PrevPath;
        //Routing.RegisterRoute(nameof(Views.FileManagement_PicView), typeof(Views.FileManagement_PicView));
        //await Shell.Current.GoToAsync(nameof(Views.FileManagement_PicView));

        mvm.UseThumbNail = !mvm.UseThumbNail;
        displayListOrView();

        await mvm.readAllFileList(mvm.PrevPath, mvm.NASFiles.Count);
        //mvm.ResetImageSrc(mvm.UseThumbNail).GetAwaiter().GetResult();
    }

    private void displayListOrView()
    {
        if (mvm.UseThumbNail)
        {
            IconChar01.Text = "\ue91a";
            //InformationBoard.IsVisible = true;

            if (listTemplate == null)
            {
                listTemplate = NASFileList.ItemTemplate;
                listLayout = (ItemsLayout)NASFileList.ItemsLayout;
            }
            if (!(Resources["picViewTemplate"] is null))
            {
                NASFileList.ItemTemplate = (DataTemplate)Resources["picViewTemplate"];
                NASFileList.ItemsLayout = gridItemsLayout;
            }
        }
        else
        {
            IconChar01.Text = "\ue919";
            NASFileList.ItemTemplate = listTemplate;
            NASFileList.ItemsLayout = listLayout;
        }

        mvm.ScrollToAnchor(ScrollToPosition.Start);
    }

    public async Task<int> Share_Tapped_Handle()
    {
        NASFileViewModel nasItem = mvm.FileSelected;
        if (nasItem == null || !IsMultiSelect)
            return -1;

        ActIndicator.IsRunning = true;
        List<ShareFile> files = new List<ShareFile>();
        int id = 0;
        foreach (NASFileViewModel item in mvm.NASFiles)
        {
            if (item.Selected)
            {
                id++;
                string localFile = Path.Combine(FileSystem.CacheDirectory, item.Name);
                if (File.Exists(localFile))
                    File.Delete(localFile);
                await mvm.fmr.NE201Download(item, localFile);
                files.Add(new ShareFile(localFile));
            }
        }
        ActIndicator.IsRunning = false;

        if (id > 0)
        {
            await Share.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "NE201",
                Files = files
            });
        }

        return id;
    }

    private async void GotoParrent_Tapped(object sender, EventArgs e)
    {
        if (mvm.PrevPath.Contains(Path.DirectorySeparatorChar.ToString()))
        {
            await mvm.ListView_GotoParentFolder(/*NASFileList*/);
        }
        HasParent = true;
    }

    private async void FileIcon_Tapped(object sender, TappedEventArgs e)
    {
        FinalLayout.IsVisible = false;
        ActIndicator.IsRunning = true;

        if (doing)
            return;

        doing = true;

        if (((NASFileViewModel)NASFileList.SelectedItem).MimeType == "folder")
        {
            //await mvm.ListView_GotoSubFolder(/*NASFileList*/);

            HasParent = true;
        }
        else
        {
            ////Routing.RegisterRoute(nameof(FileManagement_View), typeof(FileManagement_View));
            ////await Shell.Current.GoToAsync(nameof(FileManagement_View));
            if (mvm.FileSelected.MimeType.StartsWith("image"))
            {
                await Navigation.PushAsync(new View(mvm));
            }
            else
            {
                //await Navigation.PushPopupAsync(new FileManagement_Popup_SelectFile(mvm));
            }
        }

        ActIndicator.IsRunning = false;
        FinalLayout.IsVisible = true;
        doing = false;
    }

    private void Btn_Popup_MoreOptions(object sender, TappedEventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_SelectFile(mvm);
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        Shell.Current.ShowPopup(popup);
    }
}