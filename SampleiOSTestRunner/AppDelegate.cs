namespace SampleiOSTestRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using PCLTesting.Forms;
    using PCLTesting.Infrastructure;
    using Xamarin.Forms;

    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();

            window = new UIWindow(UIScreen.MainScreen.Bounds);

            var runner = new TestRunner(Assembly.GetExecutingAssembly());
            window.RootViewController = new NavigationPage(new TestRunnerPage(runner)).CreateViewController();

            window.MakeKeyAndVisible();

            return true;
        }
    }
}

