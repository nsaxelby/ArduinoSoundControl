using System;
using System.Text;

namespace ArduinoVolumeLib
{
    class Util
    {
        public static string VolumeToRow2Bars(float inputVol)
        {
            char blankChar = '*';
            string toReturn = "";
            // Number of bars we need : 12
            float barIncrement = 100F / 12F;
            if (inputVol >= 1)
            {
                toReturn += blankChar;
            }
            else
            {
                toReturn += ' ';
            }

            for (int i = 2; i <= 12; i++)
            {
                if (inputVol >= i * barIncrement)
                {
                    toReturn += blankChar;
                }
                else
                {
                    toReturn += ' ';
                }
            }

            toReturn += ' ';
            toReturn += inputVol.ToString("000");
            var caa = toReturn.ToCharArray();
            return toReturn;
        }

        public static String NormalizeNameForRow(string inputDevice)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= 15; i++)
            {
                if (inputDevice.Length >= i + 1)
                {
                    sb.Append(inputDevice[i]);
                }
                else
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
    }
}
