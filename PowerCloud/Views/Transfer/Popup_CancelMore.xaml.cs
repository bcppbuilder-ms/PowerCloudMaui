namespace PowerCloud.Views.Transfer;


public partial class Popup_CancelMore : CommunityToolkit.Maui.Views.Popup
{
    public Popup_CancelMore()
	{
		InitializeComponent();

    }


    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}