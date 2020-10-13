using System;
namespace os_project
{
    public class Word
    {
        const int WORD_SIZE = 8; 
        int address;
        string value;

        public Word(int data_address, string data_value)
        {
            System.Console.WriteLine(data_value);
            if (data_value.Remove(0, 2).Length != WORD_SIZE
                && !data_value.Remove(0, 2).Contains("x"))
                throw new System.Exception($"Invalid Word size, expected {WORD_SIZE}, was {data_value.Length}");

            this.value = data_value.Remove(0, 2);
        }

        public int Address
        {
            get { return this.address; }
        }

        public string Value
        {
            get { return this.value; }
        }

        public string ToString()
        {
            return string.Format(
                "Word: Address - {0} | " + 
                "Value - {1}", 
                this.Address,
                this.Value
            );
        }
    }
}