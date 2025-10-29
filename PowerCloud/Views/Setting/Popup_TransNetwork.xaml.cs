namespace PowerCloud.Views.Setting;


public partial class Popup_TransNetwork : CommunityToolkit.Maui.Views.Popup
{
    public Popup_TransNetwork()
	{
		InitializeComponent();

        //PowerCloud ÀÉ®×ºÞ²z 2«ö¶s
        
        DesiredSize = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 180);

    }

    //Btn_Clicked_ClosePopup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();



}