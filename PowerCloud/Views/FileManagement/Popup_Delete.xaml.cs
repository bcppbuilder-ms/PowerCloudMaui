namespace PowerCloud.Views.FileManagement;


public partial class Popup_Delete : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Delete()
	{
		InitializeComponent();

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}