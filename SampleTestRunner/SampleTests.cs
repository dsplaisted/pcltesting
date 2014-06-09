namespace SampleTestRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    [TestClass]
    public class SampleTests
    {
        [TestMethod]
        public async Task PassingTest()
        {
            await Task.Delay(250);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task FailingTest()
        {
            await Task.Delay(250);
            Assert.IsTrue(false);
        }
    }
}
