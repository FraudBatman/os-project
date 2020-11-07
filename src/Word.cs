using System;
namespace os_project
{
    public class Word
    {
        public const int WORD_SIZE = 8;
        string value;

        public Word()
        {
            this.value = "00000000";
        }

        public Word(string data_value)
        {
            if (data_value.Length == WORD_SIZE && !data_value.Contains("x"))
            {
                value = data_value;
            }
            else if (data_value.Remove(0, 2).Length == WORD_SIZE
             && !data_value.Remove(0, 2).Contains("x"))
            {
                value = data_value.Remove(0, 2);
            }
            else
                throw new System.Exception($"Invalid Word size, expected {WORD_SIZE}, was {data_value.Length}");
        }

        public string Value
        {
            get { return value; }
            set
            {
                if (value.Length == WORD_SIZE && !value.Contains("x"))
                {
                    this.value = value;
                }
                else if (value.Remove(0, 2).Length == WORD_SIZE
                 && !value.Remove(0, 2).Contains("x"))
                {
                    this.value = value.Remove(0, 2);
                }
                else
                    throw new System.Exception($"Invalid Word size, expected {WORD_SIZE}, was {value.Length}");
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
            return new Word(Utilities.WordFill(newValue));
        }

        public static Word operator -(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt - b.ValueAsInt);
            return new Word(Utilities.WordFill(newValue));
        }

        public static Word operator *(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt * b.ValueAsInt);
            return new Word(Utilities.WordFill(newValue));
        }

        public static Word operator /(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt / b.ValueAsInt);
            return new Word(Utilities.WordFill(newValue));
        }

        public static Word operator |(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt | b.ValueAsInt);
            return new Word(Utilities.WordFill(newValue));
        }

        public static Word operator &(Word a, Word b)
        {
            if (a == null || b == null)
                return null;
            string newValue = Utilities.DecToHex(a.ValueAsInt & b.ValueAsInt);
            return new Word(Utilities.WordFill(newValue));
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