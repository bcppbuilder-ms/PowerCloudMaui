
using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_Delete : CommunityToolkit.Maui.Views.Popup
{
    public Popup_Delete(MainNasFileViewModel mVm)
	{
		InitializeComponent();

    }

    //Btn_Clicked_ClosePopup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();

}