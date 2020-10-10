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

}
