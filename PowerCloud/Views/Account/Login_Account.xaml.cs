using PowerCloud.NasHttp;
using PowerCloud.ViewModels;

namespace PowerCloud.Views.Account;

public partial class Login_Account : ContentPage
{
	public Login_Account()
	{
		InitializeComponent();

        MainViewModel viewModel = App.PC2ViewModel;
        BindingContext = App.PC2ViewModel;
        if (viewModel.UserSelected != null)
        {
            entryAccount.Text = viewModel.UserSelected.UserName;
            entrySecret.Text = viewModel.UserSelected.UserSecret;
        }
    }

    string TmpIp = App.PC2ViewModel.TmpIp;

    //Btn View__101_Btn_View_FileManagement_ListView
    private async void Btn_View_FileManagement_ListView(object sender, EventArgs e)
    {
        //Routing.RegisterRoute(nameof(Views.FileManagement.ListView), typeof(Views.FileManagement.ListView));
        //await Shell.Current.GoToAsync(nameof(Views.FileManagement.ListView));


        MainViewModel viewModel = App.PC2ViewModel;

        if (string.IsNullOrEmpty(entryAccount.Text))
            entryAccount.Text = "richard1104";
        if (viewModel.TmpIp == "1.powernas.com.tw" && entryAccount.Text.StartsWith("Eleanor Roos"))
        {
            entryAccount.Text = "richard1104";
            viewModel.TmpIp = "ite2demowin10.powernas.com.tw";
        }
        if (string.IsNullOrEmpty(entrySecret.Text) || entrySecret.Text == "richard1104")
            entrySecret.Text = "12699488";

        bool loginOk = await NE201Login.Login(viewModel.TmpIp, entryAccount.Text, entrySecret.Text);

        if (loginOk)
        {
            AccountViewModel y = App.PC2ViewModel.UserSelected;
            NE201FileManager NE201 = NE201FileManager.FileManagerFactory(App.PC2ViewModel.UserSelected);
            string s = await NE201.NE201SystemInfo();

            //await Shell.Current.GoToAsync($"//{nameof(FileManagementPublic)}");
            int n = App.PC2ViewModel.accountManager.Accounts.Count;
            await Shell.Current.GoToAsync($"//{nameof(FileManagement.filesPublic)}");
            //await Shell.Current.GoToAsync($"//{nameof(FileManagement.ListView)}");
        }
        else
        {
            await DisplayAlert("登入失敗", "請重新輸入", "OK");
            await Shell.Current.GoToAsync($"//{nameof(Login_AccountList)}");
            //await Shell.Current.GoToAsync($"//{nameof(Logout_AccountList)}");
        }
    }





}