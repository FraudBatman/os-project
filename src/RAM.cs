public static class RAM
{
    /*CONSTANTS*/
    const int RAM_SIZE = 1024; //RAM size in words

    /*MEMBERS*/
    static Word[] data = new Word[RAM_SIZE];

    /*MEMORY_METHOD*/
    /// <summary>
    /// Memory method, used for both reading and writng words
    /// </summary>
    /// <param name="rw">RWFlag, read or write</param>
    /// <param name="address">Formatted 0xHHHH, where HHHH is hex</param>
    /// <param name="writeWord">Word to be written if write is called, otherwise null</param>
    /// <returns>The word read or written.</returns>
    static Word Memory(RWFlag rw, string address, Word writeWord = null)
    {


        return null;
    }
}

public enum RWFlag
{
    Read, Write
}
