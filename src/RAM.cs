using System;
using System.Text.RegularExpressions;

namespace os_project
{
    public static class RAM
    {
        #region Constants
        const int RAM_SIZE = 1024; //RAM size in words
        static Regex ADDRESS_PATTERN = new Regex("0x[0-9a-fA-f]{4}", RegexOptions.Compiled); //REGEX expression of address pattern
        #endregion

        #region Members
        static Word[] data = new Word[RAM_SIZE];
        #endregion

        #region Memory Method
        /// <summary>
        /// Memory method, used for both reading and writng words
        /// </summary>
        /// <param name="rw">RWFlag, read or write</param>
        /// <param name="address">Formatted 0xHHHH, where HHHH is hex</param>
        /// <param name="writeWord">Word to be written if write is called, otherwise null</param>
        /// <returns>The word read or written.</returns>
        public static Word Memory(RWFlag rw, string address, Word writeWord = null)
        {
            //pre-flight checklist

            //address needs to be proper format
            if (!ADDRESS_PATTERN.IsMatch(address))
            {
                throw new Exception($"Improperly formatted address {address}");
            }

            //writeWord must be null if reading
            if (rw == RWFlag.Read && writeWord != null)
            {
                throw new Exception("R/W Flag is set to read, but a write value is given.");
            }

            //writeWord can't be null if writing
            if (rw == RWFlag.Write && writeWord == null)
            {
                throw new Exception("Can not write null word");
            }

            //make sure address is valid size while also converting the address to decimal for index
            int addressIndex = convertAddress(address);
            if (addressIndex < 0 || addressIndex >= RAM_SIZE)
            {
                throw new Exception($"Address {address} is out of bounds. Converts to {addressIndex}, must be between 0 and {RAM_SIZE - 1}");
            }

            //pre-flight checklist complete!
            //begin reading and writing
            if (rw == RWFlag.Read)
            {
                return data[addressIndex];
            }
            else
            {
                data[addressIndex] = writeWord;
                return writeWord;
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Address string to decimal
        /// </summary>
        /// <param name="address">Address string</param>
        /// <returns>decimal value of address</returns>
        private static int convertAddress(string address)
        {
            return Utilities.HexToDec(address.Substring(2));
        }
        #endregion

    }

    public enum RWFlag
    {
        Read, Write
    }
}