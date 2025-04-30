using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_Add : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Add()
	{
		InitializeComponent();

        //PowerCloud ÀÉ®×ºÞ²z 3«ö¶s
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 220);


    }

    //private void Btn_Popup_AddFolder(object sender, EventArgs e)
    //{
    //    //var popup = new PopupTestContentView();
    //    var popup = new Popup_AddFolder();
    //    //popup-end
    //    //popup.VerticalOptions = LayoutAlignment.End;
    //    //popup.HorizontalOptions = LayoutAlignment.Fill;

    //    //popup-center
    //    popup.VerticalOptions = LayoutAlignment.Center;
    //    popup.HorizontalOptions = LayoutAlignment.Fill;

    //    this.ShowPopup(popup);
    //}


    //Btn_Clicked_ClosePopup
    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}