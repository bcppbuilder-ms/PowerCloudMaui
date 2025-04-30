using UIKit;

namespace PowerCloud;

public class Program
{
	// This is the main entry point of the application.
	static void Main(string[] args)
	{
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		var x = typeof(AppDelegate);
        UIApplication.Main(args, null, typeof(AppDelegate));
	}
}