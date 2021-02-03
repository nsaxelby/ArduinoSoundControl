using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using ArduinoVolumeLib;

namespace ArduinoVolumeConsole.Tests
{
    [TestClass()]
    public class UtilTests
    {
        public static readonly string blankChar = "*";

        [TestMethod()]
        public void VolumeToRow2BarsTest_1()
        {
            Assert.AreEqual(Util.VolumeToRow2Bars(0F), "             000");
        }

        [TestMethod()]
        public void VolumeToRow2BarsTest_2()
        {
            Assert.AreEqual(Util.VolumeToRow2Bars(1F), GetBlankChars(1) + "            001");
        }

        [TestMethod()]
        public void VolumeToRow2BarsTest_3()
        {
            Assert.AreEqual(Util.VolumeToRow2Bars(10F), GetBlankChars(1) + "            010");
            Assert.AreEqual(Util.VolumeToRow2Bars(17F), GetBlankChars(2) + "           017");
        }

        [TestMethod()]
        public void VolumeToRow2BarsTest_4()
        {
            Assert.AreEqual(Util.VolumeToRow2Bars(20F), GetBlankChars(2) + "           020");
            Assert.AreEqual(Util.VolumeToRow2Bars(25F), GetBlankChars(3) + "          025");

        }

        [TestMethod()]
        public void VolumeToRow2BarsTest_5()
        {
            // 11 * black squares, and one blank square
            Assert.AreEqual(Util.VolumeToRow2Bars(99F), GetBlankChars(11) + "  099");
        }

        [TestMethod()]
        public void VolumeToRow2BarsTest_6()
        {
            // 12 * black squares
            Assert.AreEqual(Util.VolumeToRow2Bars(100F), GetBlankChars(12) + " 100");
        }

        public string GetBlankChars(int numberOfBlankChars)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < numberOfBlankChars; i++)
            {
                sb.Append(blankChar);
            }
            return sb.ToString();
        }

        [TestMethod()]
        public void NormalizeNameForRow_Shorten()
        {
            Assert.AreEqual(Util.NormalizeNameForRow("123456789123456789"), "1234567891234567");
        }

        [TestMethod()]
        public void NormalizeNameForRow_Lengthen()
        {
            Assert.AreEqual(Util.NormalizeNameForRow("1234567"), "1234567         ");
        }
    }
}