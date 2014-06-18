namespace PCLTesting.Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Validation;
    using Xamarin.Forms;

    internal class TestResultPage : ContentPage
    {
        public TestResultPage(Test test)
        {
            Requires.NotNull(test, "test");

            this.BindingContext = test;
            this.Title = test.Name;

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection{
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowDefinitions = new RowDefinitionCollection{
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star)},
                },
                Padding = new Thickness(10),
            };

            var title = new Label
            {
                Font = Font.SystemFontOfSize(NamedSize.Large),
            };
            title.SetBinding<Test>(Label.TextProperty, vm => vm.Name);
            grid.Children.Add(title);

            var result = new Label();
            result.SetBinding<Test>(Label.TextProperty, vm => vm.FullFailureExplanation);

            var scrollViewer = new ScrollView
            {
                Content = result,
            };
            scrollViewer.SetValue(Grid.RowProperty, 1);
            grid.Children.Add(scrollViewer);

            this.Content = grid;
        }
    }
}
