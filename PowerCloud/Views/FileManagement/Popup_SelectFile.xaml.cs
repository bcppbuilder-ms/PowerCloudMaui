using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_SelectFile : CommunityToolkit.Maui.Views.Popup
{
    public Popup_SelectFile(MainNasFileViewModel mvm)
	{
		InitializeComponent();

        //���k��n���� �����׹L��
        //PowerCloud �ɮ׺޲z 6���s
        //FileInfo = $"{mvm.FileSelected.Size}, {mvm.FileSelected.MediaType}";

        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));
        BindingContext = mvm;
    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

}