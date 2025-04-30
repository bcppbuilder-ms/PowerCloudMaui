namespace PowerCloud.Views.FileManagement;


public partial class Popup_AddFolder : CommunityToolkit.Maui.Views.Popup
{
    public Popup_AddFolder()
	{
		InitializeComponent();

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}