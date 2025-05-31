using CommunityToolkit.Maui.Views;
using PowerCloud.ViewModels;

using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace PowerCloud.Views.BackupView;

public partial class BackupList : ContentPage
{
	public BackupList()
	{
		InitializeComponent();
        BindingContext = new BackupFrontListViewModel();
    }

    private void Btn_Popup_Backup_Transfer(object sender, EventArgs e)
    {
        //var popup = new PopupTestContentView();
        var popup = new Popup_Transfer();
        //popup-end
        //popup.VerticalOptions = LayoutAlignment.End;
        //popup.HorizontalOptions = LayoutAlignment.Fill;

        //popup-center
        popup.VerticalOptions = LayoutAlignment.Center;
        popup.HorizontalOptions = LayoutAlignment.Fill;

        this.ShowPopup(popup);
    }




}