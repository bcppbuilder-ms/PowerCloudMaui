namespace PowerCloud.Views.FileManagement;


public partial class Popup_Copy : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Copy()
	{
		InitializeComponent();
        
        //左右剛好視窗 高度0.9
        DesiredSize = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 0.9 * (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density));

    }

    //Btn_Clicked_ClosePopup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();

}