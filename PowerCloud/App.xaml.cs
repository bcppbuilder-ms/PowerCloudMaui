using PowerCloud.ViewModels;

namespace PowerCloud
{
    public partial class App : Application
    {
        public static MainViewModel PC2ViewModel { get; internal set; }
        public static string InitPath = "public";

        public App(MainViewModel mv)
        {
            InitializeComponent();
            if (App.PC2ViewModel == null)
                App.PC2ViewModel = mv;

            MainPage = new AppShell();
        }
    }
}
