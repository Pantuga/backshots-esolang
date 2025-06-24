using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackshotsEsolang.Parsing;
using static BackshotsEsolang.Interpreting.Interpreter;

namespace BackshotsEsolang.Interpreting
{
    public enum DefaultMethods
    {
        Declare = 1000,
        Assign,

        Negative,
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulus,
        Power,

        Not,
        And,
        Or,
        XOr,

        Print,
        Read,
        GetInput
    }
}
