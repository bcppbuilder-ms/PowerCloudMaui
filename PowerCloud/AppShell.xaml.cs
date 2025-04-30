namespace PowerCloud
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }


        //Btn View__021_Btn_View_Switch_AccountList_可切換的帳號列表
        private async void Btn_View_Switch_AccountList(object sender, EventArgs e)
        {
            Routing.RegisterRoute(nameof(Views.Account.Switch_AccountList), typeof(Views.Account.Switch_AccountList));
            await Shell.Current.GoToAsync(nameof(Views.Account.Switch_AccountList));
        }

        //Btn View__031_Btn_View_Logout_AccountList_登出頁 – 可選擇其他已登入帳號列表---用View__011__Login_AccountList
        private async void Btn_View_Login_AccountList(object sender, EventArgs e)
        {
            //Routing.RegisterRoute(nameof(Views.FileManagement.ListView), typeof(Views.FileManagement.ListView));
            //await Shell.Current.GoToAsync(nameof(Views.FileManagement.ListView));
            await Shell.Current.GoToAsync("//Login_AccountList");
        }
    }
}
