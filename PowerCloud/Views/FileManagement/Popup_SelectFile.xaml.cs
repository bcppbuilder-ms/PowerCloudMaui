namespace PowerCloud.Views.FileManagement;


public partial class Popup_SelectFile : CommunityToolkit.Maui.Views.Popup
{
    public Popup_SelectFile()
	{
		InitializeComponent();

        //���k��n���� �����׹L��
        //PowerCloud �ɮ׺޲z 6���s
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}