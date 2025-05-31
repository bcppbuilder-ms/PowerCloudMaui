using CommunityToolkit.Maui.Views;
using PowerCloud.ViewModels;

using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views;

public partial class TestPage : ContentPage
{
	public TestPage(MainViewModel mv)
	{
        // when vs updates, and the following line is compiled error saying "Cast invalid"
        // try to remove the emulator and recreate it again.
        InitializeComponent();
	}

    private async void Btn_View_FileManagement_View(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.FileManagement.View), typeof(Views.FileManagement.View));
        await Shell.Current.GoToAsync(nameof(Views.FileManagement.View));
    }
    
    private async void Btn_View_Login_AccountList(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_AccountList), typeof(Views.Account.Login_AccountList));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_AccountList));
    }

        
    private async void Btn_View_Login_AccountList_More(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_AccountList_More), typeof(Views.Account.Login_AccountList_More));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_AccountList_More));
    }                

    private async void Btn_View_Login_NAS(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_NAS), typeof(Views.Account.Login_NAS));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_NAS));
    }        

    private async void Btn_View_Login_NAS_IPList(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_NAS_IPList), typeof(Views.Account.Login_NAS_IPList));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_NAS_IPList));
    }        

    private async void Btn_View_Login_Account(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Login_Account), typeof(Views.Account.Login_Account));
        await Shell.Current.GoToAsync(nameof(Views.Account.Login_Account));
    }        

    private async void Btn_View_Switch_AccountList(object sender, EventArgs e)
    {
        Routing.RegisterRoute(nameof(Views.Account.Switch_AccountList), typeof(Views.Account.Switch_AccountList));
        await Shell.Current.GoToAsync(nameof(Views.Account.Switch_AccountList));
    }


    private void OnCounterClicked(object sender, EventArgs e)
    {
        var popup = new PopupTestContentView();
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }


}