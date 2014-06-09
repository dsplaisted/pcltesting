//  Log the test results to Debug output even when compiling for release mode
#define DEBUG

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCLTesting.Infrastructure
{
    public class TestRunner
    {
        private readonly ObservableCollection<Test> tests;
        private readonly StringBuilder log = new StringBuilder();

        public TestRunner(IEnumerable<Test> tests)
        {
            this.tests = new ObservableCollection<Test>(tests);
        }

        public TestRunner(params Assembly[] assemblies)
            : this(assemblies.SelectMany(a => new TestDiscoverer().DiscoverTests(a)))
        {
        }

        public ObservableCollection<Test> Tests
        {
            get { return this.tests; }
        }

        public int TestCount { get { return this.tests.Count; } }
        public int PassCount { get; private set; }
        public int FailCount { get; private set; }

        public string Log { get { return this.log.ToString(); } }

        public async Task RunTestsAsync(IEnumerable<Test> testList = null, IProgress<TestRunProgress> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (testList == null)
            {
                testList = this.Tests;
            }
            else
            {
                testList = testList.ToList(); // snapshot the enumerable.
            }

            this.PassCount = 0;
            this.FailCount = 0;

            this.log.Length = 0;

            progress.ReportIfNotNull(new TestRunProgress(this.TestCount));
            foreach (var test in testList)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Give the app some breathing room to process a cancel button,
                // since otherwise running tests is only as async as the tests themselves.
                await TaskEx.Yield();

                await test.RunAsync();
                if (test.Result == TestState.Passed)
                {
                    this.PassCount++;
                }
                else if (test.Result == TestState.Failed)
                {
                    this.FailCount++;
                    this.LogLine("Failed: " + test.FullName);
                    this.LogLine(test.FailureException.ToString());
                    this.LogLine("");
                }
                else
                {
                    throw new InvalidOperationException("Unexpected test state: " + test.Result);
                }

                progress.ReportIfNotNull(new TestRunProgress(this.PassCount, this.FailCount, this.TestCount));
            }

            this.LogLine("");
            this.LogLine(PassCount.ToString() + " passed, " + FailCount + " failed, " + TestCount + " total");
        }

        void LogLine(string s)
        {
            Debug.WriteLine(s);
            this.log.AppendLine(s);
        }
    }
}
