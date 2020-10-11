using System;
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
<<<<<<< HEAD

    /// <summary>
    /// Removes characters at the specified range (e.g. [0, 1]), if not index
    /// is specified, remove the first element in the instruct
    /// </summary>
    /// <param name="instruct">Instruction string</param>
    /// <param name="range">Start index = [0], count = [1]</param>
    /// <returns>String array object of instruction</returns>
    public static string RemoveCharacters(string instruct, int[] range = null)
    {   
        if(range == null)
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
}
=======
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
}
>>>>>>> 3f43e0af7184f8acdf47977cd3bcfcd455aa1976
