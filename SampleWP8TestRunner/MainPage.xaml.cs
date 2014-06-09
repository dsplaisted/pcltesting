namespace SampleTestRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Phone.Controls;
    using PCLTesting.Forms;
    using PCLTesting.Infrastructure;
    using Xamarin.Forms;

    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var runner = new TestRunner(Assembly.GetExecutingAssembly());

            Forms.Init();
            this.Content = new NavigationPage(new TestRunnerPage(runner)).ConvertPageToUIElement(this);
        }
    }
}
