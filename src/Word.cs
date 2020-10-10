public class Word
{
    /*CONSTANTS*/
    const int WORD_SIZE = 4;    //word size in bytes

    /*CONSTRUCTORS*/
    public Word()
    {
        data = new byte[WORD_SIZE];
    }

    /*MEMBERS*/
    private byte[] data = new byte[WORD_SIZE];

    /*PROPERTIES*/
    public byte[] Data
    {
        get { return data; }
        set { data = value; }
    }
}