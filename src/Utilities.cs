using System;
public static class Utilities
{

    // Utility functions that will convert instructions to usable data

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static int HexToDec(string hex)
    {
        return Convert.ToInt32(hex, 16);
    }

}