namespace PowerCloud.Views.FileManagement;


public partial class Popup_Rename : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Rename()
	{
		InitializeComponent();

    }

    //Btn_Clicked_ClosePopup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();


}