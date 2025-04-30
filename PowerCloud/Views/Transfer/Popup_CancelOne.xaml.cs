namespace PowerCloud.Views.Transfer;


public partial class Popup_CancelOne : CommunityToolkit.Maui.Views.Popup
{
    public Popup_CancelOne()
	{
		InitializeComponent();

    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();
}