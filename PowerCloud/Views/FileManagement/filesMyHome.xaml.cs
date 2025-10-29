using CommunityToolkit.Maui.Views;
using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;

public partial class filesMyHome : ContentPage
{
    public filesMyHome()
    {
        InitializeComponent();

        myControl.OnHasParentChanged += MyControl_OnHasParentChanged;
        actIndicator = (ActivityIndicator)myControl.FindByName("ActIndicator");
        //setupToolItems();
    }

    private void MyControl_OnHasParentChanged(object? sender, EventArgs e)
    {
        //updateToolbarItems();
    }

    #region Setup ToolbarItems & OnBackButtonPressed & OnAppearing
    List<ToolbarItem> singleToolItems = new List<ToolbarItem>();
    List<ToolbarItem> multiToolItems = new List<ToolbarItem>();

    private void setupToolItems()
    {
        return; //暫時不使用 ToolbarItems
    }

    protected override bool OnBackButtonPressed()
    {
        MainNasFileViewModel mvm = (MainNasFileViewModel)myControl.BindingContext;
        if (mvm.PrevPath.Contains(Path.DirectorySeparatorChar.ToString()))
        {
            GotoParentFolder(mvm);
            return true;
        }
        else
            return base.OnBackButtonPressed();
    }
    private async void GotoParentFolder(MainNasFileViewModel mvm)
    {
        await mvm.ListView_GotoParentFolder();
        myControl.HasParent = true;
        updateToolbarItems();
    }

    private void updateToolbarItems()
    {
        return; //暫時不使用 ToolbarItems
    }

    AccountViewModel currentUser = null;

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
            myControl.InitPath = "home";
            currentUser = App.PC2ViewModel.UserSelected;
        }
        if (myControl.BindingContext != null)
        {
            MainNasFileViewModel mvm = (MainNasFileViewModel)myControl.BindingContext;
            if (mvm.NASFiles.Count == 0)
            {
                actIndicator.IsRunning = true;
                await mvm.readFileList(mvm.PrevPath, 1, 0, actIndicator);
            }
            else
            {
                actIndicator.IsRunning = false;
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
    #endregion

    ActivityIndicator actIndicator;

    private async void Btn_Popup_FileManDelete(object sender, EventArgs e)
    {
        //await PushPopupAsync(new FileManagement_Popup_Delete((MainNasFileViewModel)myControl.BindingContext));
        await AppShell.Current.ShowPopupAsync(new Popup_Delete((MainNasFileViewModel)myControl.BindingContext));
    }

    private /*async*/ void Btn_MultiSelect_ListView(object sender, EventArgs e)
    {
        myControl.IsMultiSelect = !myControl.IsMultiSelect;

        setupToolItems();


        ////string prm = "?ite2path=" + HttpUtility.UrlEncode(mvm.PrevPath); //傳遞參數
        //string prm = "?ite2path=" + ((MainNasFileViewModel)myControl.BindingContext).ScrollHighest; //傳遞參數

        //App.PC2ViewModel.MultiSelectVM = (MainNasFileViewModel)myControl.BindingContext;

        ////FileManagement_MultiSelect_ListView newPage = new FileManagement_MultiSelect_ListView();
        ////Routing.RegisterRoute(nameof(Views.FileManagement_MultiSelect_ListView) + prm, newPage.GetType());

        //Routing.RegisterRoute(nameof(Views.FileManagement_MultiSelect_ListView) + prm, typeof(Views.FileManagement_MultiSelect_ListView));
        //await Shell.Current.GoToAsync(nameof(Views.FileManagement_MultiSelect_ListView) + prm);
    }

    private async void ToolbarItem_GotoParent_Clicked(object sender, EventArgs e)
    {
        await ((MainNasFileViewModel)myControl.BindingContext).ListView_GotoParentFolder(/*NASFileList*/);
        myControl.HasParent = true;
        updateToolbarItems();
    }

    private async void ToolItem_Download_Clicked(object sender, EventArgs e)
    {
        MainNasFileViewModel mvm = (MainNasFileViewModel)myControl.BindingContext;
        if (mvm == null)
            return;

        if (myControl.IsMultiSelect)
        {
            if (mvm.AddFiles() > 0)
            {
                //await Navigation.PushPopupAsync(new Transfer_Popup_CancelMore(mvm));
                App.PC2ViewModel.MultiSelectVM = mvm;
                ////Routing.RegisterRoute(nameof(Transfer_Download), typeof(Transfer_Download));
                ////await Shell.Current.GoToAsync(nameof(Transfer_Download));
            }
        }
        else
        {
            await mvm.DownloadFile(mvm.FileSelected);
            await DisplayAlert("Download File", "File download have been done.", "Close");
        }
    }

    private async void ToolItem_Rename_Clicked(object sender, EventArgs e)
    {
        MainNasFileViewModel mvm = (MainNasFileViewModel)myControl.BindingContext;
        if (mvm == null)
            return;

        if (mvm.FileSelected == null)
        {
            return;
        }
        //((NASFileViewModel)NASFileList.SelectedItem).Selected = true;
        mvm.FileSelected.Selected = true;
        //await Navigation.PushPopupAsync(new FileManagement_Popup_Rename(mvm)); //(NASFileViewModel)NASFileList.SelectedItem));
    }

    private void ToolbarItem_Upload_Clicked(object sender, EventArgs e)
    {
    }

    private async void ShareFiles_Clicked(object sender, EventArgs e)
    {
        await myControl.Share_Tapped_Handle();
    }

    private async void Btn_Select_More_Clicked(object sender, EventArgs e)
    {
        await myControl.FilesSelectMore(sender, e);
    }
}