namespace PowerCloud.Views.Account;

public partial class Switch_AccountList : ContentPage
{
	public Switch_AccountList()
	{
		InitializeComponent();
    }


    //View__013__Btn_View_Login_NAS
    private async void Btn_View_Login_NAS(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_NAS), typeof(Views.Account.Login_NAS));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_NAS));
    }

}