namespace PowerCloud.Views.FileManagement;


public partial class Popup_Move : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Move()
	{
		InitializeComponent();

        //���k��n���� ����0.9
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 0.9 * (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density));

    }


    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}