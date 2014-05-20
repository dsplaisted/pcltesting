﻿//  Log the test results to Debug output even when compiling for release mode
#define DEBUG

using System;
using System.Collections.Generic;
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
        List<Test> _tests;
        StringBuilder _log;

        public TestRunner(IEnumerable<Test> tests)
        {
            _tests = tests.ToList();
            _log = new StringBuilder();
        }

        public TestRunner(params Assembly[] assemblies)
            : this(assemblies.SelectMany(a => new TestDiscoverer().DiscoverTests(a)))
        {
        }

        public int TestCount { get { return _tests.Count; } }
        public int PassCount { get; private set; }
        public int FailCount { get; private set; }
        public int SkipCount { get; private set; }

        public string Log { get { return _log.ToString(); } }

        public async Task RunTestsAsync(IProgress<TestRunProgress> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            PassCount = 0;
            FailCount = 0;
            SkipCount = 0;

            _log.Length = 0;

            progress.ReportIfNotNull(new TestRunProgress(this.TestCount));
            foreach (var test in _tests)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Give the app some breathing room to process a cancel button,
                // since otherwise running tests is only as async as the tests themselves.
                await TaskEx.Yield();

                await test.RunAsync();
                if (test.TestState == TestState.Passed)
                {
                    PassCount++;
                }
                else if (test.TestState == TestState.Skipped)
                {
                    SkipCount++;
                    LogLine("Skipped: " + test.FullName);
                }
                else if (test.TestState == TestState.Failed)
                {
                    FailCount++;
                    LogLine("Failed: " + test.FullName);
                    LogLine(test.FailureException.ToString());
                    LogLine("");
                }
                else
                {
                    throw new InvalidOperationException("Unexpected test state: " + test.TestState);
                }

                progress.ReportIfNotNull(new TestRunProgress(this.PassCount, this.FailCount, this.SkipCount, this.TestCount));
            }

            LogLine("");
            LogLine(PassCount.ToString() + " passed, " + FailCount + " failed, " + SkipCount + " skipped, " + TestCount + " total");
        }

        void LogLine(string s)
        {
            Debug.WriteLine(s);
            _log.AppendLine(s);
        }
    }
}