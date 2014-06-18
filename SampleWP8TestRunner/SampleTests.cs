namespace SampleTestRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using PCLTesting;

    [TestClass]
    public class SampleTests
    {
        private static int figityTestCounter;

        [TestMethod]
        public async Task PassingTest()
        {
            await Task.Delay(100);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task FailingTest()
        {
            await Task.Delay(100);
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void FigityTest()
        {
            Assert.AreEqual(0, figityTestCounter++ % 2);
        }
    }
}
