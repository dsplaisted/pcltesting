namespace PCLTesting.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PCLTesting.Infrastructure;
    using Validation;
    using Xamarin.Forms;

    public class TestRunnerPage : ContentPage
    {
        public TestRunnerPage(TestRunner runner)
            : this(new TestRunnerViewModel(runner))
        {
        }

        public TestRunnerPage(TestRunnerViewModel viewModel)
        {
            Requires.NotNull(viewModel, "viewModel");

            this.Title = "Tests";
            this.BindingContext = viewModel;
            var panel = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                },
                Padding = new Thickness(10),
            };

            var testsList = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var cell = new TextCell();
                    cell.SetBinding<Test>(TextCell.TextProperty, t => t.Name);
                    cell.SetBinding<Test>(TextCell.DetailProperty, t => t.OneLineFailureExplanation);
                    return cell;
                }),
            };
            testsList.ItemTapped += async (s, e) =>
            {
                Test test = (Test)e.Item;
                if (test.Result == TestState.Failed)
                {
                    await this.Navigation.PushAsync(new TestResultPage(test));
                }
            };
            testsList.SetBinding<TestRunnerViewModel>(ListView.ItemsSourceProperty, vm => vm.Tests);
            var scrollViewer = new ScrollView { Content = testsList };
            scrollViewer.SetValue(Grid.ColumnSpanProperty, 3);
            panel.Children.Add(scrollViewer);

            var startButton = new Button();
            startButton.SetValue(Grid.ColumnSpanProperty, 3);
            startButton.SetValue(Grid.RowProperty, 1);
            startButton.SetBinding<TestRunnerViewModel>(Button.CommandProperty, vm => vm.ToggleRunCommand);
            startButton.SetBinding<TestRunnerViewModel>(Button.TextProperty, vm => vm.IsRunning, converter: new BooleanToggleConverter<string>("Stop", "Run tests"));
            panel.Children.Add(startButton);

            var progress = new ProgressBar();
            progress.SetValue(Grid.ColumnSpanProperty, 3);
            progress.SetValue(Grid.RowProperty, 2);
            progress.SetBinding<TestRunnerViewModel>(ProgressBar.IsVisibleProperty, vm => vm.IsRunning);
            progress.SetBinding<TestRunnerViewModel>(ProgressBar.ProgressProperty, vm => vm.CurrentProgress, converter: new ProgressValueConverter());
            panel.Children.Add(progress);

            var passLabel = new Label { Text = "Pass" };
            passLabel.SetValue(Grid.RowProperty, 3);
            passLabel.SetValue(Grid.ColumnProperty, 0);
            panel.Children.Add(passLabel);
            var passValueLabel = new Label();
            passValueLabel.SetBinding<TestRunnerViewModel>(Label.TextProperty, vm => vm.CurrentProgress, converter: new NestedValueConverter(vm => vm.PassCount, false));
            passValueLabel.SetValue(Grid.RowProperty, 4);
            passValueLabel.SetValue(Grid.ColumnProperty, 0);
            panel.Children.Add(passValueLabel);
            var passPercentLabel = new Label();
            passPercentLabel.SetBinding<TestRunnerViewModel>(Label.TextProperty, vm => vm.CurrentProgress, converter: new NestedValueConverter(vm => vm.PassCount, true));
            passPercentLabel.SetValue(Grid.RowProperty, 5);
            passPercentLabel.SetValue(Grid.ColumnProperty, 0);
            panel.Children.Add(passPercentLabel);

            var failLabel = new Label { Text = "Fail" };
            failLabel.SetValue(Grid.RowProperty, 3);
            failLabel.SetValue(Grid.ColumnProperty, 1);
            panel.Children.Add(failLabel);
            var failValueLabel = new Label();
            failValueLabel.SetBinding<TestRunnerViewModel>(Label.TextProperty, vm => vm.CurrentProgress, converter: new NestedValueConverter(vm => vm.FailCount, false));
            failValueLabel.SetValue(Grid.RowProperty, 4);
            failValueLabel.SetValue(Grid.ColumnProperty, 1);
            panel.Children.Add(failValueLabel);
            var failPercentLabel = new Label();
            failPercentLabel.SetBinding<TestRunnerViewModel>(Label.TextProperty, vm => vm.CurrentProgress, converter: new NestedValueConverter(vm => vm.FailCount, true));
            failPercentLabel.SetValue(Grid.RowProperty, 5);
            failPercentLabel.SetValue(Grid.ColumnProperty, 1);
            panel.Children.Add(failPercentLabel);

            var totalLabel = new Label { Text = "Total" };
            totalLabel.SetValue(Grid.RowProperty, 3);
            totalLabel.SetValue(Grid.ColumnProperty, 2);
            panel.Children.Add(totalLabel);
            var totalValueLabel = new Label();
            totalValueLabel.SetBinding<TestRunnerViewModel>(Label.TextProperty, vm => vm.CurrentProgress, converter: new NestedValueConverter(vm => vm.TestCount, false));
            totalValueLabel.SetValue(Grid.RowProperty, 4);
            totalValueLabel.SetValue(Grid.ColumnProperty, 2);
            panel.Children.Add(totalValueLabel);

            this.Content = panel;
        }

        private class ProgressValueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var progress = (TestRunProgress)value;
                double result = progress.TestCount == 0 ? 0 : (double)progress.ExecuteCount / progress.TestCount;
                return result;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class NestedValueConverter : IValueConverter
        {
            private readonly Func<TestRunProgress, int> viewModelFetcher;
            private readonly bool showPercent;

            internal NestedValueConverter(Func<TestRunProgress, int> viewModelFetcher, bool showPercent)
            {
                Requires.NotNull(viewModelFetcher, "viewModelFetcher");

                this.viewModelFetcher = viewModelFetcher;
                this.showPercent = showPercent;
            }

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var progress = (TestRunProgress)value;
                int intValue = this.viewModelFetcher(progress);
                if (showPercent)
                {
                    double percent = (double)intValue / progress.TestCount;
                    return double.IsNaN(percent)
                        ? string.Empty
                        : string.Format("{0:n0}%", percent * 100);
                }
                else
                {
                    return intValue.ToString(culture);
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class BooleanToggleConverter<T> : IValueConverter
        {
            private readonly T trueValue;
            private readonly T falseValue;

            internal BooleanToggleConverter(T trueValue, T falseValue)
            {
                this.trueValue = trueValue;
                this.falseValue = falseValue;
            }

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return (bool)value ? this.trueValue : this.falseValue;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
