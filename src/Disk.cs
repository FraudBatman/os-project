using System;
using System.Text.RegularExpressions;

namespace os_project
{
    public class Disk
    {
        #region Constants
        const int DISK_SIZE = 2048; //Disk size in words
        static Regex ADDRESS_PATTERN = new Regex("0x[0-9a-fA-f]{4}", RegexOptions.Compiled); //REGEX expression of address pattern
        #endregion

        #region Members
        static Word[] data = new Word[DISK_SIZE];
        #endregion

        #region Storage Method
        /// <summary>
        /// Storage method, used for both reading and writng words
        /// </summary>
        /// <param name="rw">RWFlag, read or write</param>
        /// <param name="address">Formatted 0xHHHH, where HHHH is hex</param>
        /// <param name="writeWord">Word to be written if write is called, otherwise null</param>
        /// <returns>The word read or written.</returns>
        public static Word Storage(RWFlag rw, string address, Word writeWord = null)
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
            if (addressIndex < 0 || addressIndex >= DISK_SIZE)
            {
                throw new Exception($"Address {address} is out of bounds. Converts to {addressIndex}, must be between 0 and {DISK_SIZE - 1}");
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

        #region Public Functions
        /// <summary>
        /// Reads a chunk of the disk and returns all it reads
        /// </summary>
        /// <param name="startAddr">Address to start reading at, formatted 0xHHHH, where HHHH is hex</param>
        /// <param name="length">Total amount of words to read</param>
        /// <returns>Word array of length given with all words read.</returns>
        public static Word[] ReadBulk(string startAddr, uint length)
        {
            //pre-flight checklist

            //address needs to be proper format
            if (!ADDRESS_PATTERN.IsMatch(startAddr))
            {
                throw new Exception($"Improperly formatted address {startAddr}");
            }

            //make sure address is valid size while also converting the address to decimal for index
            int addressIndex = convertAddress(startAddr);
            if (addressIndex < 0 || addressIndex >= DISK_SIZE)
            {
                throw new Exception($"Address {startAddr} is out of bounds. Converts to {addressIndex}, must be between 0 and {DISK_SIZE - 1}");
            }

            //make sure last address is a valid index, this is a more project-friendly OOB catcher
            int lastIndex = addressIndex + ((int)length - 1); //length is -1 since addressIndex is the first
            if (lastIndex >= DISK_SIZE)
            {
                throw new Exception($"ReadBulk from ${startAddr} of length {length} will go out of bounds of the disk.");
            }

            //pre-flight checklist complete!
            //fill an array with deeta
            Word[] returnValue = new Word[length];
            for (int i = 0; i < length; i++)
            {
                returnValue[i] = data[addressIndex + i]; //right side will increment with i
            }

            return returnValue;
        }
        #endregion

        #region Private Functions
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
}