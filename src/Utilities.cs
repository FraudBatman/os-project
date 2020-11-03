using System;
using System.Diagnostics;

namespace os_project
{
    public static class Utilities
    {

        // Utility functions that will convert instructions to usable data

        /// <summary>
        /// Converts hex as string to decimal
        /// </summary>
        /// <param name="hex">string of hex value</param>
        /// <returns>decimal value of hex</returns>
        public static int HexToDec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        /// <summary>
        /// Removes characters at the specified range (e.g. [0, 1]), if not index
        /// is specified, remove the first element in the instruct
        /// </summary>
        /// <param name="instruct">Instruction string</param>
        /// <param name="range">Start index = [0], count = [1]</param>
        /// <returns>String array object of instruction</returns>
        public static string RemoveCharacters(string instruct, int[] range = null)
        {
            if (range == null)
            {
                range[0] = 0;
                range[1] = 0;
            }

            // remove special characters at the specified range
            if (range[1] == 0)
                return instruct.Substring(range[0]);
            else
                return instruct.Substring(range[0], range[1]);
        }

        public static string HexToBin(string hex)
        {
            return Convert.ToString(Convert.ToInt32(hex, 16), 2);
        }
        public static string BinToHex(string bin)
        {
            return Convert.ToInt32(bin, 2).ToString("X");
        }
        public static int BinToDec(string bin)
        {
            return Convert.ToInt32(bin, 2);
        }
        public static string DecToBin(int dec)
        {
            return Convert.ToString(dec, 2);
        }
        public static string DecToHex(int dec)
        {
            return BinToHex(DecToBin(dec));
        }
        public static int[] parseControlCard(string controlCard)
        {
            string trimmedString;

            if (controlCard.Contains("JOB"))
                trimmedString = controlCard.Replace("JOB ", "");
            else
                trimmedString = controlCard.Replace("Data ", "");

            var strArr = trimmedString.Split(' ');
            int[] intArr = new int[3];
            int i = 0;
            foreach (var hex in strArr)
            {
                intArr[i] = HexToDec(hex);
                i++;
            }
            return intArr;
        }

        /// <summary>
        /// Converts a hex value into the proper size and format required to be read as a word
        /// </summary>
        /// <param name="hexValue">The hex value to convert</param>
        /// <returns>A word formatted properly</returns>
        public static string WordFill(string hexValue)
        {
            string format = "0x";

            //add leading 0's
            for (int i = 1; i < (Word.WORD_SIZE / 2) - hexValue.Length; i++)
            {
                format += "0";
            }

            return format + hexValue;
        }

        #region Timers
        static Stopwatch timer;
        public static Stopwatch Timer
        {
            get { return timer; }
        }

        public static void StartTimer() 
        { 
            // Start the timer
            return;
        }

        public static void StopTimer()
        {
            // We want to convert this to milliseconds
            return;
        }

        #endregion
    }
}

