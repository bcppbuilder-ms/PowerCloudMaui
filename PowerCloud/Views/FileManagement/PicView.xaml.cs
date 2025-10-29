using CommunityToolkit.Maui.Extensions;
using PowerCloud.ViewModels;
using System.Collections.ObjectModel;

namespace PowerCloud.Views.FileManagement;

public partial class PicView : ContentPage
{
	public PicView()
	{
		InitializeComponent();

        if (string.IsNullOrEmpty(InitPath))
            InitPath = App.InitPath;

        mvm = new MainNasFileViewModel()
        {
            CurrentMaxPage = 0,
            PrevPath = InitPath,
            NASFiles = new ObservableCollection<NASFileViewModel>(),
            PageCollectionView = NASFileList
        };
        BindingContext = mvm;
        InformationBoard.BindingContext = this;
        GridBottom.BindingContext = this;

        //if (DeviceInfo.Platform == DevicePlatform.iOS)
        //    ThisRefreshView.HeightRequest = 600;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (App.PC2ViewModel.UserSelected == null || App.PC2ViewModel.UserSelected.AccessToken == "unknown")
        {
            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    Routing.RegisterRoute("GoLogin_AccountList", typeof(Views.Login_AccountList));
            //    await Shell.Current.GoToAsync("GoLogin_AccountList");
            //}
            //else
            //{
            //    await Shell.Current.GoToAsync($"//{nameof(Views.Login_AccountList)}");
            //    //Shell.Current.FlyoutIsPresented = false;
            //    //return;

            //    //await Shell.Current.Navigation.PopToRootAsync();
            //    //return;
            //}
            return;
        }


        if (currentUser != App.PC2ViewModel.UserSelected)
        {
            InitPath = "public";
            currentUser = App.PC2ViewModel.UserSelected;
        }
        if (BindingContext != null)
        {
            MainNasFileViewModel mvm = (MainNasFileViewModel)BindingContext;
            if (mvm.NASFiles.Count == 0)
            {
                ActIndicator.IsRunning = true;
                await mvm.readFileList(mvm.PrevPath, 1, 0, ActIndicator);
            }
            else
            {
                ActIndicator.IsRunning = false;
            }
            if (App.PC2ViewModel.UserSelected != null /*&& 
                    App.PC2ViewModel.UserSelected.AccessToken != App.PC2ViewModel.accountManager.SelectedAccessToeken*/)
            {
                App.PC2ViewModel.accountManager.SelectedAccessToeken = App.PC2ViewModel.UserSelected.AccessToken;
                App.PC2ViewModel.accountManager.SelectedNasAddress = App.PC2ViewModel.UserSelected.UserNasLink;
                App.PC2ViewModel.accountManager.SelectedRefreshToken = App.PC2ViewModel.UserSelected.RefreshToken;
            }
        }
        else
        {
            throw new Exception("BindinContext is null.");
        }
    }

    private void Btn_Popup_FileManSort(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_Sort();
        //popup-end
        popup.VerticalOptions = LayoutOptions.End;
        popup.HorizontalOptions = LayoutOptions.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        AppShell.Current.ShowPopup(popup);
    }


    private void Btn_Popup_FileManAdd(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_Add(mvm);
        //popup-end
        popup.VerticalOptions = LayoutOptions.End;
        popup.HorizontalOptions = LayoutOptions.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }

    private void Btn_Popup_FileManSelectFile(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_More(mvm);
        //popup-end
        popup.VerticalOptions = LayoutOptions.End;
        popup.HorizontalOptions = LayoutOptions.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }

    ItemsLayout listLayout;
    DataTemplate listTemplate;
    GridItemsLayout gridItemsLayout = new GridItemsLayout(2, ItemsLayoutOrientation.Vertical) { VerticalItemSpacing = 5 };



    AccountViewModel currentUser = null;

    /*
     * import from xamarin
     */
    MainNasFileViewModel mvm;

    public event EventHandler OnHasParentChanged;



    /*
     * Property : InitPath
     */
    public static readonly BindableProperty InitPathProperty = BindableProperty.Create(
        nameof(InitPath), typeof(string), typeof(PicView),
        defaultValue: string.Empty, defaultBindingMode: BindingMode.OneWay, propertyChanged: InitPathPropertyChanged);

    private static void InitPathPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        PicView control = (PicView)bindable;
        if (oldValue != null && oldValue.ToString() != newValue.ToString())
        {
            //((MainNasFileViewModel)control.BindingContext).PrevPath = newValue?.ToString();
        }
    }

    public string InitPath
    {
        get { return base.GetValue(InitPathProperty)?.ToString(); }
        set
        {
            string s = GetValue(InitPathProperty)?.ToString();
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
        nameof(IsMultiSelect), typeof(bool), typeof(PicView),
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
        nameof(HasParent), typeof(bool), typeof(PicView),
        defaultValue: false, defaultBindingMode: BindingMode.OneWay, propertyChanged: HasParentPropertyChanged);

    private static void HasParentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue)
        {
            PicView control = (PicView)bindable;
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



    //private void thumb_pic_Tapped(object sender, TappedEventArgs e)
    //{
    //    //App.PC2ViewModel.PicViewPath = mvm.PrevPath;
    //    //Routing.RegisterRoute(nameof(Views.FileManagement_PicView), typeof(Views.FileManagement_PicView));
    //    //await Shell.Current.GoToAsync(nameof(Views.FileManagement_PicView));

    //    mvm.UseThumbNail = !mvm.UseThumbNail;
    //    displayListOrView();

    //    //await mvm.readAllFileList(mvm.PrevPath, mvm.NASFiles.Count);
    //    mvm.ResetImageSrc(mvm.UseThumbNail, mvm.ShowTwoColumn).GetAwaiter().GetResult();
    //}

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
            if (Resources.ContainsKey("picViewTemplate"))
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
}