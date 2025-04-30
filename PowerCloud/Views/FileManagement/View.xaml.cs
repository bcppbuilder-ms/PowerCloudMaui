using CommunityToolkit.Maui.Views;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views.FileManagement;

public partial class View : ContentPage
{
	public View()
	{
		InitializeComponent();


    }


    private void Btn_Popup_FileManViewDelete(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_ViewDelete();
        //popup-end
        //popup.VerticalOptions = LayoutAlignment.End;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        popup.VerticalOptions = LayoutAlignment.Center;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }

}