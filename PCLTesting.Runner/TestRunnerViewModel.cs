using System;
using System.Collections.Generic;
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
            this.runner = runner;
            var startCommand = new StartCommandImpl(this);
            this.StartCommand = startCommand;
            this.StopCommand = new CancelCommand(startCommand);
        }

        private TestRunProgress currentProgress;
        public TestRunProgress CurrentProgress
        {
            get { return this.currentProgress; }
            set { this.SetProperty(ref this.currentProgress, value); }
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
                var progress = new Microsoft.Progress<TestRunProgress>(value => this.viewModel.CurrentProgress = value);
                await this.viewModel.runner.RunTestsAsync(progress, cancellationToken);
            }
        }
    }
}