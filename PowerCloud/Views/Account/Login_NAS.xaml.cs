using PowerCloud.ViewModels;

namespace PowerCloud.Views.Account;

public partial class Login_NAS : ContentPage
{
	public Login_NAS()
	{
		InitializeComponent();

        MainViewModel mv = App.PC2ViewModel;
        if (mv.UserSelected != null) {
            mv.TmpIp = mv.UserSelected.UserNasLink;
            entryTmpIp.Text = mv.TmpIp;
        }

        if (!string.IsNullOrEmpty(App.PC2ViewModel.LoginMessage))
            labelLogout.Text = App.PC2ViewModel.LoginMessage;
    }

    //Btn View__014_Btn_View_Login_NAS_IPList
    private async void Btn_View_Login_NAS_IPList(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_NAS_IPList), typeof(Views.Account.Login_NAS_IPList));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_NAS_IPList));
    }

    //Btn View__015_Btn_View_Login_Account
    private async void Btn_View_Login_Account(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(entryTmpIp.Text) || string.IsNullOrWhiteSpace(entryTmpIp.Text))
        {
            await DisplayAlert("Wanning", "NAS Ip/name can not be empty.", "Close");
            return;
        }

        entryTmpIp.Text = entryTmpIp.Text.Trim();

        // ¤U¤@¨B
        if (!entryTmpIp.Text.Contains("."))
            entryTmpIp.Text += ".powernas.com.tw";

        App.PC2ViewModel.TmpIp = entryTmpIp.Text;
        App.PC2ViewModel.LoginMessage = string.Empty;
        Routing.RegisterRoute(nameof(Views.Account.Login_Account), typeof(Views.Account.Login_Account));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_Account));
    }
}