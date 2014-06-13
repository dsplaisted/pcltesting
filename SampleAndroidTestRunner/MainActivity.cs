namespace SampleAndroidTestRunner
{
    using System;
    using System.Reflection;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Runtime;
    using Android.Views;
    using Android.Widget;
    using PCLTesting.Runner;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    [Activity(Label = "SampleAndroidTestRunner", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var runner = new TestRunner(Assembly.GetExecutingAssembly());

            Xamarin.Forms.Forms.Init(this, bundle);
            SetPage(new NavigationPage(new TestRunnerPage(runner)));
        }
    }
}

