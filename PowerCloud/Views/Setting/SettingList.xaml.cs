using CommunityToolkit.Maui.Views;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views.Setting;

public partial class SettingList : ContentPage
{
	public SettingList()
	{
		InitializeComponent();
	}


    private void Btn_Popup_Setting_TransNumber(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_TransNumber();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }

        private void Btn_Popup_Setting_TransNetwork(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_TransNetwork();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }

        private void Btn_Popup_Setting_BackupNetwork(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_BackupNetwork();
        //popup-end
        popup.VerticalOptions = LayoutAlignment.End;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        //popup.VerticalOptions = LayoutAlignment.Center;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }


}