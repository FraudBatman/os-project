using System;
namespace os_project
{
    public class Word
    {
        public const int WORD_SIZE = 8;
        int ownerOfWord;
        string value;

        public Word()
        {
            this.value = "00000000";
        }


        public Word(int ownerOfWord)
        {
            this.value = "00000000";
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
            set
            {
                if (value.Substring(0, 2) == "0x")
                {
                    this.value = value.Remove(0, 2);
                }
                else
                    this.value = value;
            }
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
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt + b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator -(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt - b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator *(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt * b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator /(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt / b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator |(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt | b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        public static Word operator &(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt & b.ValueAsInt);
            return new Word(a.OwnerOfWord, Utilities.WordFill(newValue));
        }

        #endregion

        public override string ToString()
        {
            return string.Format(
                "Value - {0}",
                Value
            );
        }
    }
}