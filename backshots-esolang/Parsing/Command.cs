using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackshotsEsolang.Lexing;

namespace BackshotsEsolang.Parsing
{
    public enum CommandType
    {
        Literal,
        Identifier,
        Function,
        Array
    }
    public readonly struct Command(uint line, Command[] args, CommandType cmdType, int? value = null)
    {
        public readonly  CommandType Type = cmdType;
        public readonly Command[] Arguments = args;
        public readonly int? Value = value;
        public readonly uint Line = line;

        public override string ToString()
        {
            return ToString("");
        }
        public readonly string ToString(string indent)
        {
            const string AddIndent = "  ";
            string inIndent = indent + AddIndent;
            StringBuilder sb = new();

            sb.AppendLine(indent + "{");
            sb.AppendLine(inIndent + "Type: " + Type);
            if (Value != null) sb.AppendLine(inIndent + "Value: " + Value);

            if (Arguments.Length != 0)
            {
                sb.AppendLine(inIndent + "Arguments: ");

                foreach (var arg in Arguments)
                {
                    sb.Append(arg.ToString(inIndent));
                }
            }
            sb.AppendLine(indent + "},");

            return sb.ToString();
        }
    }
}
