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