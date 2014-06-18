# PCL Testing

PCL Testing is a simple test framework for testing across .NET platforms.
It supports .NET Framework 4.5, Windows Phone 8, Windows Store apps,
Silverlight 5, Xamarin.iOS, and Xamarin.Android. 

[Portable Class Libraries][1] (PCLs) make it easy to write code for multiple
platforms, but there's not currently a great way to test a PCL on each
platform it targets.  Hopefully test frameworks such as
[xUnit](https://xunit.codeplex.com/) and MSTest will soon support
cross-platform testing with PCLs.  In the meantime, PCL Testing provides
basic support for writing portable tests and running them on multiple
platforms.

[1]: http://msdn.microsoft.com/en-us/library/gg597391(v=vs.110).aspx

## How to install?

[Always Be NuGetting](https://nuget.org/packages/PCLTesting/).

## Usage
PCL Testing includes MSTest-style test attributes and asserts, and a simple
test runner which can discover and run tests.  You can put your tests in a
Portable Class Library, and create apps for each platform you support which
reference the PCL tests and provide UI to run them and display the results.

Because the included test runner includes only very basic functionality, I
recommend using MSTest on platforms where it is supported (ie .NET Framework
and Windows Store apps).  To do this, create MSTest projects which include
your test source code via file linking.  The test attributes and asserts
are in different namespaces for PCL Testing, MSTest, and MSTest for Windows
Store apps, but PCL testing includes dummy types in the MSTest namespaces
so in your test source code you can simply add using statements for all
the namespaces, as follows:

```cs
using PCLTesting;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
```

To create a test runner that runs on device, install the PCL Testing NuGet
package in a new device app, which may target any of:
 1. Windows Phone 8.x Silverlight (no Windows Phone 8.1 Appx support yet)
 2. Xamarin.Android
 3. Xamarin.iOS

Then follow these steps for each device project. In each case, consider
replacing the argument to the `TestRunner` constructor to point to the
assemblies that contain your unit tests.

### Windows Phone

Add this to your `MainPage.xaml.cs` file:

    using PCLTesting.Runner;
    using Xamarin.Forms;

Replace your `MainPage` constructor with this:

    public MainPage()
    {
        this.InitializeComponent();
        Forms.Init();

        var runner = new TestRunner(Assembly.GetExecutingAssembly());
        this.Content = new NavigationPage(new TestRunnerPage(runner)).ConvertPageToUIElement(this);
    }

### Android

Add this to your `MainActivity.cs` file:

    using PCLTesting.Runner;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

Replace your `MainActivity` class with this:

    // Preserve the Activity attribute that you already had.
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Forms.Forms.Init(this, bundle);

            var runner = new TestRunner(Assembly.GetExecutingAssembly());
            SetPage(new NavigationPage(new TestRunnerPage(runner)));
        }
    }

### iOS

Add this to your `AppDelegate.cs` file:

    using PCLTesting.Runner;
    using Xamarin.Forms;

Replace your `FinishedLaunching` method with this:

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        window = new UIWindow(UIScreen.MainScreen.Bounds);
        Forms.Init();

        var runner = new TestRunner(Assembly.GetExecutingAssembly());
        window.RootViewController = new NavigationPage(new TestRunnerPage(runner)).CreateViewController();

        window.MakeKeyAndVisible();
        return true;
    }

For a full example of how to use PCL Testing, see the samples in the source
code behind [PCL Testing](https://github.com/dsplaisted/pcltesting/).