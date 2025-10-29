using Microsoft.Maui.Controls;

namespace PowerCloud.Views.BackupView;


public partial class Popup_Transfer : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Transfer()
	{
		InitializeComponent();

    }

    //Btn_Clicked_ClosePopup
    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();

}