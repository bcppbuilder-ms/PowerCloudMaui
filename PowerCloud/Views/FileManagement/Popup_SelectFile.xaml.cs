namespace PowerCloud.Views.FileManagement;


public partial class Popup_SelectFile : CommunityToolkit.Maui.Views.Popup
{
    public Popup_SelectFile()
	{
		InitializeComponent();

        //左右剛好視窗 但高度過高
        //PowerCloud 檔案管理 6按鈕
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}