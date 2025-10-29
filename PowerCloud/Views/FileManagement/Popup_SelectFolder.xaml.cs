using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_SelectFolder : CommunityToolkit.Maui.Views.Popup
{
    public Popup_SelectFolder(MainNasFileViewModel mvm)
	{
		InitializeComponent();

        //PowerCloud ÀÉ®×ºÞ²z 4«ö¶s
        DesiredSize = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 280);

    }

    //Btn_Clicked_ClosePopup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();


}