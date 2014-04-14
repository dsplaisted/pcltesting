namespace PCLTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public struct TestRunProgress
    {
        public TestRunProgress(int totalCount)
            : this()
        {
            this.TestCount = totalCount;
        }

        public TestRunProgress(int passCount, int failCount, int skipCount, int testCount)
            : this()
        {
            this.PassCount = passCount;
            this.FailCount = failCount;
            this.SkipCount = skipCount;
            this.TestCount = testCount;
        }

        public int PassCount { get; private set; }

        public int FailCount { get; private set; }

        public int SkipCount { get; private set; }

        public int TestCount { get; private set; }
    }
}
