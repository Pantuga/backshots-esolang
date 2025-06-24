using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BackshotsEsolang.Parsing;

namespace BackshotsEsolang.Interpreting
{
    public class Interpreter
    {
        private readonly Queue<char> InputQueue = [];
        private readonly Dictionary<int, double> Variables = [];
        private readonly Dictionary<int, Func<Command[], double>> Functions;

        public Interpreter()
        {
            Functions = new()
            {
                { (int)DefaultMethods.Declare,  args => DeclareVar(GetIdentifier(args[0])) },
                { (int)DefaultMethods.Assign,   args => AssignVar(GetIdentifier(args[0]), GetValue(args[1])) },

                { (int)DefaultMethods.Negative, args => - GetValue(args[0]) },
                { (int)DefaultMethods.Add,      args => GetValue(args[0]) + GetValue(args[1]) },
                { (int)DefaultMethods.Subtract, args => GetValue(args[0]) - GetValue(args[1]) },
                { (int)DefaultMethods.Multiply, args => GetValue(args[0]) * GetValue(args[1]) },
                { (int)DefaultMethods.Divide,   args => GetValue(args[0]) / GetValue(args[1]) },
                { (int)DefaultMethods.Modulus,  args => GetValue(args[0]) % GetValue(args[1]) },
                { (int)DefaultMethods.Power,    args => Math.Pow(GetValue(args[0]), GetValue(args[1])) },

                { (int)DefaultMethods.Not,      args => BoolToNum(!NumToBool(GetValue(args[0]))) },
                { (int)DefaultMethods.And,      args => BoolToNum(NumToBool(GetValue(args[0])) && NumToBool(GetValue(args[1]))) },
                { (int)DefaultMethods.Or,       args => BoolToNum(NumToBool(GetValue(args[0])) || NumToBool(GetValue(args[1]))) },
                { (int)DefaultMethods.XOr,      args => BoolToNum(NumToBool(GetValue(args[0])) ^ NumToBool(GetValue(args[1]))) },

                { (int)DefaultMethods.Print,    args => { Print(args); return 1; } },
                { (int)DefaultMethods.Read,     args => ReadLine() },
                { (int)DefaultMethods.GetInput, args => InputQueue.Dequeue() },
            };

        }

        private  void Print(Command[] args)
        {
            Array.ForEach(args, a => Console.Write(GetString(a)));
        }
        private int ReadLine()
        {
            string? input = Console.ReadLine();
            if (input == null) return 0;
            Array.ForEach(input.ToArray(), ch => InputQueue.Enqueue(ch));
            return 1;
        }
        private static bool NumToBool(double val)
        {
            return val == 0;
        }
        private static int BoolToNum(bool bl)
        {
            return bl ? 1 : 0;
        }
        private double GetValue(Command cmd)
        {
            int value = cmd.Value ?? 0;
            switch (cmd.Type)
            {
                case CommandType.Literal:
                    return value;

                case CommandType.Identifier:
                    return Variables[value];

                case CommandType.Function:
                    return Functions[value].Invoke(cmd.Arguments);

                case CommandType.Array:
                    throw new ArgumentException($"Line {cmd.Line}: Expected single value, got array");

                default: return 0; // unreachable
            };
        }
        private int GetIdentifier(Command cmd)
        {
            if (cmd.Type != CommandType.Identifier) 
                throw new ArgumentException($"Line {cmd.Line}: Expected identifier, got '{cmd.Type}'");

            return cmd.Value ?? 0;
        }
        private string GetString(Command cmd)
        { 
            if (cmd.Type == CommandType.Array)
            {
                StringBuilder sb = new();
                foreach (Command arg in cmd.Arguments ) sb.Append(GetString(arg));
                return sb.ToString();
            }
            else return $"{(char)GetValue(cmd)}";
        }

        private double DeclareVar(int name, double value = 0)
        {
            Variables.Add(name, value);
            return value;
        }
        private double AssignVar(int name, double value)
        {
            Variables[name] = value;
            return value;
        }

        public Argument Evaluate(Command cmd)
        {
            switch (cmd.Type)
            {
                case CommandType.Array:
                    List<Argument> output = [];
                    Array.ForEach(cmd.Arguments, arg => output.Add(Evaluate(arg)));
                    return new ArgumentArray([..output]);

                case CommandType.Literal:
                    return new ArgumentDouble(cmd.Value ?? 0);

                case CommandType.Function:
                    return new ArgumentDouble(Functions[cmd.Value ?? 0].Invoke(cmd.Arguments));

                case CommandType.Identifier:
                    return new ArgumentDouble(Variables[cmd.Value ?? 0]);

                default: return new ArgumentDouble(0); // unreachable
            }
        }

        public void Execute(Command[] cmds)
        {
            foreach (Command cmd in cmds)
            {
                Evaluate(cmd);
            }
        }
    }
}
