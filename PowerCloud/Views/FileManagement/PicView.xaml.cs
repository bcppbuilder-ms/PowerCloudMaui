using CommunityToolkit.Maui.Views;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views.FileManagement;

public partial class PicView : ContentPage
{
	public PicView()
	{
		InitializeComponent();
	}

    private void Btn_Popup_FileManSort(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_Sort();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }


    private void Btn_Popup_FileManAdd(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_Add();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }

    private void Btn_Popup_FileManSelectFile(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_SelectFile();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }


}