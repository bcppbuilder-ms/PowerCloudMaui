namespace PowerCloud.Views.Setting;


public partial class Popup_TransNetwork : CommunityToolkit.Maui.Views.Popup
{
    public Popup_TransNetwork()
	{
		InitializeComponent();

        //PowerCloud �ɮ׺޲z 2���s
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 180);

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();



}