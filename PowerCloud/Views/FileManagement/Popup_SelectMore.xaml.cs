namespace PowerCloud.Views.FileManagement;


public partial class Popup_SelectMore : CommunityToolkit.Maui.Views.Popup
{
    public Popup_SelectMore()
	{
		InitializeComponent();

        //PowerCloud ÀÉ®×ºÞ²z 5«ö¶s
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 340);

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}