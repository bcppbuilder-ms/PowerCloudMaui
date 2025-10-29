using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_More : CommunityToolkit.Maui.Views.Popup
{
    public Popup_More(MainNasFileViewModel mvm)
	{
		InitializeComponent();

        //���k��n���� �����׹L��
        //PowerCloud �ɮ׺޲z 6���s
        //FileInfo = $"{mvm.FileSelected.Size}, {mvm.FileSelected.MediaType}";

        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));
        Size = new(0.95 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));
        BindingContext = mvm;
    }

    //Btn_Clicked_ClosePopup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();

    private async void Btn_Popup_More_DownloadFile(object sender, TappedEventArgs e)
    {
        MainNasFileViewModel? mvm = BindingContext as MainNasFileViewModel;

        if (mvm?.FileSelected != null)
        {
            bool result = await mvm.DownloadFile(mvm.FileSelected);
            if (result)
                await AppShell.Current.CurrentPage.DisplayAlert("�U������", "�ɮפw���\�U��", "OK");
            else
                await AppShell.Current.CurrentPage.DisplayAlert("�U������", "�еy��A��", "OK");
        }
    }
}