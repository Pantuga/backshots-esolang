using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BackshotsEsolang.Lexing
{
    public static class Lexer
    {
        public static readonly char[] AllowedChars = ['n', 'm', 'g', 'h'];
        public static readonly char[] WhiteSpace = [' ', '\t', '\n'];
        public const char OpenComment = '#';
        public const char CloseComment = '$';
        public static readonly Dictionary<char, AllowedChar> AllowedCharsPairs = new()
            {
                {'n', AllowedChar.N },
                {'m', AllowedChar.M },
                {'g', AllowedChar.G },
                {'h', AllowedChar.H },
            };

        private static AllowedChar GetAllowedChar(char ch, uint line)
        {
            if (AllowedCharsPairs.TryGetValue(ch, out AllowedChar value))
                return value;

            throw new Exception($"Line {line}: The character '{ch}' is not valid");
        }

        private static string SanitizeCode(string code)
        {
            return code.ToLower().Replace("\r\n", "\n");
        }

        public static Token[] Tokenize(string code)
        {
            uint line = 1;
            List<Token> output = [];

            string pCode = SanitizeCode(code);
            for (int i = 0; i < pCode.Length; i++)
            {
                if (WhiteSpace.Contains(pCode[i])) 
                {

                    while ((i < pCode.Length) && WhiteSpace.Contains(pCode[i])) 
                    {
                        // Console.WriteLine($"'{pCode[i]}'({(int)pCode[i]}) is white space (i = {i})");
                        if (pCode[i] == '\n') line++;
                        i++;
                    }
                    i--;
                }
                else
                {
                    if (pCode[i] == OpenComment)
                    {
                        while ((i < pCode.Length) && !(pCode[i] == '\n' || pCode[i] == CloseComment)) i++;
                        if ((i < pCode.Length) && pCode[i] == '\n') line++;
                    }
                    else
                    {
                        output.Add(new Token([], line));
                        while ((i < pCode.Length) && !WhiteSpace.Contains(pCode[i]))
                        {
                            // Console.WriteLine($"'{pCode[i]}'({(int)pCode[i]}) is not white space (i = {i})");
                            output[^1].AddChar(GetAllowedChar(pCode[i], line));
                            i++;
                        }
                        i--;
                    }
                }
            }


            return [..output];
        }
    }
}
