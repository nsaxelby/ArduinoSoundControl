using ArduinoVolumeLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArduinoVolumeServiceTests
{
    [TestClass()]
    public class LoggerTests
    {
        [TestMethod()]
        public void AddNoOverflow()
        {
            Logger lgr = new Logger(10);
            lgr.AddLog("log1", true);
            lgr.AddLog("log2", false);
            lgr.AddLog("log3");

            string[] res = lgr.ReadLogResults();
            Assert.IsTrue(res.Length == 3);
            Assert.IsTrue(res[0].Contains("log1") && res[0].Contains("ERROR"));
            Assert.IsTrue(res[1].Contains("log2") && res[1].Contains("INFO"));
            Assert.IsTrue(res[2].Contains("log3") && res[2].Contains("INFO"));
        }

        [TestMethod()]
        public void AddWithOverflow()
        {
            Logger lgr = new Logger(3);
            lgr.AddLog("log1", true);
            lgr.AddLog("log2", false);
            lgr.AddLog("log3");
            lgr.AddLog("log4");
            lgr.AddLog("log5");

            string[] res = lgr.ReadLogResults();
            Assert.IsTrue(res.Length == 3);
            Assert.IsTrue(res[0].Contains("log3"));
            Assert.IsTrue(res[1].Contains("log4"));
            Assert.IsTrue(res[2].Contains("log5"));
        }
    }
}
