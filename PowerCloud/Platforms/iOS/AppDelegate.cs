using Foundation;
using UIKit;

namespace PowerCloud;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public override UIWindow Window { get => base.Window; set => base.Window = value; }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}