namespace PowerCloud.Views;

public partial class PopupTestContentView : CommunityToolkit.Maui.Views.Popup
{
    public PopupTestContentView()
	{
		InitializeComponent();
        //var width = DeviceDisplay.MainDisplayInfo.Width;

        //左右剛好視窗 但高度過高
        //PowerCloud 檔案管理 6按鈕
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));

        //PowerCloud 檔案管理 5按鈕
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 340);

        //PowerCloud 檔案管理 4按鈕
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 280);

        //PowerCloud 檔案管理 3按鈕
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 220);

        //左右剛好視窗 高度由後面控制
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 0.3 * (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density));


        //右邊爆出
        //Size = new(DeviceDisplay.MainDisplayInfo.Width);
    }


    //Btn_Clicked_ClosePopup
    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();
}