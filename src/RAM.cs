using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace os_project
{
    public static class RAM
    {
        /// <summary>
        /// Size of RAM in terms of words
        /// </summary>
        public const int RAM_SIZE = 1024;

        public static Word[] data = new Word[RAM_SIZE];

        public static Word Read(int index)
        {
            return data[index];
        }

        public static void Write(int index, Word value)
        {
            data[index] = value;
        }
    }
}