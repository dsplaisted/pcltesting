namespace PCLTesting.Infrastructure
{
    public struct TestRunProgress
    {
        public TestRunProgress(int totalCount)
            : this()
        {
            this.TestCount = totalCount;
        }

        public TestRunProgress(int passCount, int failCount, int testCount)
            : this()
        {
            this.PassCount = passCount;
            this.FailCount = failCount;
            this.TestCount = testCount;
        }

        public int PassCount { get; private set; }

        public int FailCount { get; private set; }

        public int TestCount { get; private set; }

        public int ExecuteCount
        {
            get { return this.PassCount + this.FailCount; }
        }
    }
}
