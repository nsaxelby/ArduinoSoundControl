using System;

namespace ArduinoVolumeLib
{
    // Simple custom in memory logger, rotates atfer x messages, just used for debugging
    public class Logger
    {
        int _curWriteLog = 0;
        string[] _logArr;
        int _curLength;
        int _maxNumRecords;

        public Logger(int numberOfRecords = 100)
        {
            this._maxNumRecords = numberOfRecords;
            this._logArr = new string[numberOfRecords];
        }

        public void AddLog(string message, bool error = false)
        {
            string msg = "[" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "] [" + (error ? "ERROR" : "INFO") + "] " + message;
            _logArr[_curWriteLog++] = msg;
            if(_curWriteLog >= _maxNumRecords)
            {
                _curWriteLog = 0;
            }

            if(_curLength < _maxNumRecords)
            {
                _curLength++;
            }
        }

        /// <summary>
        /// Reads logs that are currently stored in memory
        /// </summary>
        /// <returns>Array returned is oldest at [0]</returns>
        public string[] ReadLogResults()
        {
            string[] toReturn = new string[_curLength];
            int writeLoc = _curLength - 1;
            int curReadLoc = _curWriteLog -1;
            while(writeLoc >= 0)
            {
                if(curReadLoc < 0)
                {
                    curReadLoc = _maxNumRecords - 1;
                }
                toReturn[writeLoc--] = _logArr[curReadLoc--];
            }
            return toReturn;
        }
    }
}
