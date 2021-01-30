using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoVolumeService
{
    public static class Util
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
    }
}
