using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackshotsEsolang.Lexing;

namespace BackshotsEsolang
{

    public static class Utils
    {
        public static void PrintColored(string text, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = originalColor;
        }
        public static void LogArray<T>(T[] array) 
        {
            foreach (T item in array)
            {
                if (item == null) continue;
                Console.WriteLine(item.ToString() + ",");
            }
        }
    }
}
