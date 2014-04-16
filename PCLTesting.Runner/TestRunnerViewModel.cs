using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PCLCommandBase;
using Validation;

namespace PCLTesting.Infrastructure
{
    public class TestRunnerViewModel : BindableBase
    {
        private readonly TestRunner runner;

        public TestRunnerViewModel(TestRunner runner)
        {
            Requires.NotNull(runner, "runner");

            this.runner = runner;
            var startCommand = new StartCommandImpl(this);
            this.StartCommand = startCommand;
            this.StopCommand = new CancelCommand(startCommand);

            this.RegisterDependentProperty(() => CurrentProgress, () => Summary);
            this.RegisterDependentProperty(() => CurrentProgress, () => Log);
            this.RegisterDependentProperty(() => IsRunning, () => Log);
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
                    await this.viewModel.runner.RunTestsAsync(progress, cancellationToken);
                }
                finally
                {
                    this.viewModel.IsRunning = false;
                }
            }
        }
    }
}