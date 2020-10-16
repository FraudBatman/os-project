using System;
namespace os_project
{
    public class Word
    {
        const int WORD_SIZE = 8;
        int ownerOfWord;
        string value;

        public Word(int ownerOfWord, string data_value)
        {
            if (data_value.Remove(0, 2).Length != WORD_SIZE
                && !data_value.Remove(0, 2).Contains("x"))
                throw new System.Exception($"Invalid Word size, expected {WORD_SIZE}, was {data_value.Length}");

            this.value = data_value.Remove(0, 2);
            this.ownerOfWord = ownerOfWord;
        }

        public int OwnerOfWord
        {
            get { return this.ownerOfWord; }
        }

        public string Value
        {
            get { return this.value; }
        }

        public override string ToString()
        {
            return string.Format(
                "Word: Job Owner - {0} | " +
                "Value - {1}",
                this.ownerOfWord,
                this.Value
            );
        }
    }
}