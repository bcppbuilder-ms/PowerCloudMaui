namespace PowerCloud.Views.Setting;


public partial class Popup_TransNumber : CommunityToolkit.Maui.Views.Popup
{
    public Popup_TransNumber()
	{
		InitializeComponent();

        //PowerCloud �ɮ׺޲z 4���s
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 280);

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();


}