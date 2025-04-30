namespace PowerCloud.Views;

public partial class PopupTestContentView : CommunityToolkit.Maui.Views.Popup
{
    public PopupTestContentView()
	{
		InitializeComponent();
        //var width = DeviceDisplay.MainDisplayInfo.Width;

        //���k��n���� �����׹L��
        //PowerCloud �ɮ׺޲z 6���s
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density));

        //PowerCloud �ɮ׺޲z 5���s
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 340);

        //PowerCloud �ɮ׺޲z 4���s
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 280);

        //PowerCloud �ɮ׺޲z 3���s
        Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 220);

        //���k��n���� ���ץѫ᭱����
        //Size = new(1 * (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density), 0.3 * (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density));


        //�k���z�X
        //Size = new(DeviceDisplay.MainDisplayInfo.Width);
    }


    //Btn_Clicked_ClosePopup
    //https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/views/popup
    void Btn_Clicked_ClosePopup(object? sender, EventArgs e) => Close();
}