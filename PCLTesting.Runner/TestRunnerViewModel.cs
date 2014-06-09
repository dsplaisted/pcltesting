namespace PCLTesting.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using PCLCommandBase;
    using Validation;

    public class TestRunnerViewModel : BindableBase
    {
        private readonly TestRunner runner;
        private readonly FilteredCollectionView<Test, string> filteredTests;

        public TestRunnerViewModel(TestRunner runner)
        {
            Requires.NotNull(runner, "runner");

            this.runner = runner;
            var startCommand = new StartCommandImpl(this);
            this.StartCommand = startCommand;
            this.StopCommand = new CancelCommand(startCommand);

            this.filteredTests = new FilteredCollectionView<Test, string>(
                runner.Tests,
                (t, q) => string.IsNullOrWhiteSpace(q) || t.Name.IndexOf(q.Trim(), StringComparison.OrdinalIgnoreCase) >= 0,
                this.SearchQuery,
                new TestComparer());
            this.filteredTests.CollectionChanged += filteredTests_CollectionChanged;
            this.filteredTests.ItemChanged += filteredTests_ItemChanged;
            this.UpdateTestCounts();

            this.RegisterDependentProperty(() => CurrentProgress, () => Summary);
            this.RegisterDependentProperty(() => CurrentProgress, () => Log);
            this.RegisterDependentProperty(() => IsRunning, () => Log);
            this.RegisterDependentProperty(() => IsRunning, () => ToggleRunCommand);
        }

        private void UpdateTestCounts()
        {
            this.PassCount = this.Tests.Count(t => t.Result == TestState.Passed);
            this.FailCount = this.Tests.Count(t => t.Result == TestState.Failed);
            this.TestCount = this.Tests.Count;
        }

        private TestRunProgress currentProgress;
        public TestRunProgress CurrentProgress
        {
            get { return this.currentProgress; }
            set { this.SetProperty(ref this.currentProgress, value); }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetProperty(ref this.isRunning, value); }
        }

        private string searchQuery;
        public string SearchQuery
        {
            get { return this.searchQuery; }
            set
            {
                this.SetProperty(ref this.searchQuery, value);
                this.filteredTests.FilterArgument = value;
            }
        }

        public ICollection<Test> Tests
        {
            get { return this.filteredTests; }
        }

        private int testCount;
        public int TestCount
        {
            get { return this.testCount; }
            private set { this.SetProperty(ref this.testCount, value); }
        }

        private int passCount;
        public int PassCount
        {
            get { return this.passCount; }
            private set { this.SetProperty(ref this.passCount, value); }
        }

        private int failCount;
        public int FailCount
        {
            get { return this.failCount; }
            set { this.SetProperty(ref this.failCount, value); }
        }

        public string Summary
        {
            get
            {
                return string.Format(
                  CultureInfo.CurrentCulture,
                  "{0}/{1} tests passed ({2}%)",
                  this.CurrentProgress.PassCount,
                  this.CurrentProgress.ExecuteCount,
                  this.CurrentProgress.ExecuteCount > 0 ? (100 * this.CurrentProgress.PassCount / this.CurrentProgress.ExecuteCount) : 0);
            }
        }

        public string Log
        {
            get { return this.runner.Log; }
        }

        public ICommand StartCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        /// <summary>
        /// Gets either the <see cref="StartCommand"/> or the <see cref="StopCommand"/>
        /// depending on which one is enabled.
        /// </summary>
        public ICommand ToggleRunCommand
        {
            get { return this.IsRunning ? this.StopCommand : this.StartCommand; }
        }

        private void filteredTests_ItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Result")
            {
                this.UpdateTestCounts();
            }
        }

        private void filteredTests_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.UpdateTestCounts();
        }

        private class StartCommandImpl : CommandBase
        {
            private readonly TestRunnerViewModel viewModel;

            internal StartCommandImpl(TestRunnerViewModel viewModel)
            {
                Requires.NotNull(viewModel, "viewModel");
                this.viewModel = viewModel;
            }

            protected override async Task ExecuteCoreAsync(object parameter, CancellationToken cancellationToken)
            {
                this.viewModel.IsRunning = true;
                try
                {
                    var progress = new Microsoft.Progress<TestRunProgress>(value => this.viewModel.CurrentProgress = value);
                    await this.viewModel.runner.RunTestsAsync(this.viewModel.Tests, progress, cancellationToken);
                }
                finally
                {
                    this.viewModel.IsRunning = false;
                }
            }
        }

        private class TestComparer : IComparer<Test>
        {
            public int Compare(Test x, Test y)
            {
                int compare = string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
                if (compare != 0)
                {
                    return compare;
                }

                return string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}