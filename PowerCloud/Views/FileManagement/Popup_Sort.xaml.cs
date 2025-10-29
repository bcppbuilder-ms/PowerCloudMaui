namespace PowerCloud.Views.FileManagement;


public partial class Popup_Sort : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Sort()
	{
		InitializeComponent();

        //PowerCloud ÀÉ®×ºÞ²z 4«ö¶s
        DesiredSize = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 280);

    }

    //Btn_Clicked_ClosePopup
    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();


}