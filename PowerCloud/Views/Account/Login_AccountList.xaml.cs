using PowerCloud.ViewModels;
using System.Collections.ObjectModel;

namespace PowerCloud.Views.Account;

public partial class Login_AccountList : ContentPage
{
	public Login_AccountList()
	{
		InitializeComponent();

        //if (App.PC2ViewModel?.UserSelected != null)
        //{
        //    App.PC2ViewModel.UserSelected.AccessToken = "unknown";
        //    App.PC2ViewModel.UserSelected = null;
        //}
        //else
        //{

        //}

        Alls = App.PC2ViewModel.AllLogoutAccounts;

        //Logout_AccList_User.ItemsSource = Alls;
        ViewModel = App.PC2ViewModel;

        if (ViewModel.RecentAccounts != null) 
        {
            if (ViewModel.RecentAccounts.Count >= 3) {
                id3.AccountId = ViewModel.RecentAccounts[2].UserName;
                id3.AccountNas = ViewModel.RecentAccounts[2].SystemInfo.HostName;
            }
            if (ViewModel.RecentAccounts.Count >= 2) {
                id2.AccountId = ViewModel.RecentAccounts[1].UserName;
                id2.AccountNas = ViewModel.RecentAccounts[1].SystemInfo.HostName;
            }
            if (ViewModel.RecentAccounts.Count >= 1) {
                id1.AccountId = ViewModel.RecentAccounts[0].UserName;
                id1.AccountNas = ViewModel.RecentAccounts[0].SystemInfo.HostName;
            }
        }

        BindingContext = this;
    }

    public MainViewModel ViewModel;
    ObservableCollection<AccountViewModel> alls;
    public ObservableCollection<AccountViewModel> Alls { get { return alls; } set { alls = value; } }

    //«ö¶s¨Æ¥ó
    //Btn View__012_Btn_View_Login_AccountList_More
    private async void Btn_View_Login_AccountList_More(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_AccountList_More), typeof(Views.Account.Login_AccountList_More));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_AccountList_More));
    }

    //Btn View__013_Btn_View_Login_NAS
    private async void Btn_View_Login_NAS(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_NAS), typeof(Views.Account.Login_NAS));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_NAS));
    }
}