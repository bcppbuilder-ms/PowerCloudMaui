namespace PowerCloud.Views.FileManagement;


public partial class Popup_ViewDelete : CommunityToolkit.Maui.Views.Popup
{
    public Popup_ViewDelete()
	{
		InitializeComponent();

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}