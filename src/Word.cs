using System;
namespace os_project
{
    public class Word
    {
        /*CONSTANTS*/
        const int WORD_SIZE = 4;    //word size in bytes

        /*CONSTRUCTORS*/
        public Word()
        {
            data = new byte[WORD_SIZE];
            for (int i = 0; i < WORD_SIZE; i++)
            {
                data[i] = 0;
            }
        }

        /// <summary>
        /// Constructor for word straight from byte array, skipping some arbitrary steps
        /// </summary>
        /// <param name="input">byte array, should always be the size of the word</param>
        public Word(byte[] input)
        {
            if (input.Length != WORD_SIZE)
                throw new System.Exception($"Invalid Word size, expected {WORD_SIZE}, was {input.Length}");
            data = input;
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
}