using PowerCloud.ViewModels;

namespace PowerCloud.Views.FileManagement;


public partial class Popup_More : CommunityToolkit.Maui.Views.Popup
{
    public Popup_More(MainNasFileViewModel mvm)
	{
		InitializeComponent();

        //左右剛好視窗 但高度過高
        //PowerCloud 檔案管理 6按鈕
        //FileInfo = $"{mvm.FileSelected.Size}, {mvm.FileSelected.MediaType}";

        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));
        DesiredSize = new(0.95 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));
        BindingContext = mvm;
    }

    //Btn_Clicked_ClosePopup
    async void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => await CloseAsync();

    private async void Btn_Popup_More_DownloadFile(object sender, TappedEventArgs e)
    {
        MainNasFileViewModel? mvm = BindingContext as MainNasFileViewModel;

        if (mvm?.FileSelected != null)
        {
            bool result = await mvm.DownloadFile(mvm.FileSelected);
            if (result)
                await AppShell.Current.CurrentPage.DisplayAlert("下載完成", "檔案已成功下載", "OK");
            else
                await AppShell.Current.CurrentPage.DisplayAlert("下載失敗", "請稍後再試", "OK");
        }
    }
}