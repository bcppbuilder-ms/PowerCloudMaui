using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_MoreMulti : CommunityToolkit.Maui.Views.Popup
{
    public Popup_MoreMulti(MainNasFileViewModel mvm)
	{
		InitializeComponent();

        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));
        Size = new(0.8 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 360);
        BindingContext = mvm;
    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}