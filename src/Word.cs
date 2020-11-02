using System;
namespace os_project
{
    public class Word
    {
        public const int WORD_SIZE = 8;
        int ownerOfWord;
        string value;

        public Word(int ownerOfWord)
        {
            this.value = "null";
            this.ownerOfWord = ownerOfWord;
        }

        public Word(int ownerOfWord, string data_value)
        {
            if (data_value.Remove(0, 2).Length != WORD_SIZE
                && !data_value.Remove(0, 2).Contains("x"))
                throw new System.Exception($"Invalid Word size, expected {WORD_SIZE}, was {data_value.Length}");

            value = data_value.Remove(0, 2);
            this.ownerOfWord = ownerOfWord;
        }

        public int OwnerOfWord
        {
            get { return ownerOfWord; }
            set { ownerOfWord = value; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// The value of the word as an int for your pleasure :)
        /// </summary>
        /// <returns>The value of the word as an int for your pleasure :)</returns>
        public int ValueAsInt
        {
            get { return Utilities.HexToDec(value); }
        }

        /// <summary>
        /// Used for bitwise AND and OR operations
        /// </summary>
        /// <returns>The value as a binary number</returns>
        public string ValueAsBin
        {
            get { return Utilities.HexToBin(value); }
        }

        #region Operations

        public static Word operator +(Word a, Word b)
        {
            string newValue = Utilities.DecToHex(a.ValueAsInt + b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator -(Word a, Word b)
        {
            string newValue = Utilities.DecToHex(a.ValueAsInt - b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator *(Word a, Word b)
        {
            string newValue = Utilities.DecToHex(a.ValueAsInt * b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator /(Word a, Word b)
        {
            string newValue = Utilities.DecToHex(a.ValueAsInt / b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator |(Word a, Word b)
        {
            string newValue = Utilities.DecToHex(a.ValueAsInt | b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator &(Word a, Word b)
        {
            string newValue = Utilities.DecToHex(a.ValueAsInt & b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        #endregion

        public override string ToString()
        {
            return string.Format(
                "Word: Job Owner - {0} | " +
                "Value - {1}",
                ownerOfWord,
                Value
            );
        }
    }
}