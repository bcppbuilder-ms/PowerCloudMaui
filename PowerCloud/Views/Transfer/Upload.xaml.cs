using CommunityToolkit.Maui.Views;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;


namespace PowerCloud.Views.Transfer;

public partial class Upload : ContentPage
{
	public Upload()
	{
		InitializeComponent();
	}

    private void Btn_Popup_Transfer_CancelOne(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_CancelOne();
        //popup-end
        //popup.VerticalOptions = LayoutAlignment.End;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        popup.VerticalOptions = LayoutAlignment.Center;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }
    private void Btn_Popup_Transfer_CancelMore(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_CancelMore();
        //popup-end
        //popup.VerticalOptions = LayoutAlignment.End;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        popup.VerticalOptions = LayoutAlignment.Center;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }

}