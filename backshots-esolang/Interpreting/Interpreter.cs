using System.Text;
using BackshotsEsolang.Parsing;

namespace BackshotsEsolang.Interpreting
{
    public class Interpreter
    {
        private readonly Queue<char> InputQueue = [];
        private readonly Dictionary<int, double> NumVariables = [];
        private readonly Dictionary<int, Func<Command[], double>> Functions;
        public bool DidPrintText = false;

        public Interpreter()
        {
            Functions = new()
            {
                { (int)DefaultMethods.Declare,      args => DeclareVar(GetIdentifier(args[0])) },
                { (int)DefaultMethods.Assign,       args => AssignVar(GetIdentifier(args[0]), GetValue(args[1])) },
                { (int)DefaultMethods.Delete,       args => DeleteVar(GetIdentifier(args[0])) },

                { (int)DefaultMethods.Negative,     args => - GetValue(args[0]) },
                { (int)DefaultMethods.Add,          args => GetValue(args[0]) + GetValue(args[1]) },
                { (int)DefaultMethods.Subtract,     args => GetValue(args[0]) - GetValue(args[1]) },
                { (int)DefaultMethods.Multiply,     args => GetValue(args[0]) * GetValue(args[1]) },
                { (int)DefaultMethods.Divide,       args => GetValue(args[0]) / GetValue(args[1]) },
                { (int)DefaultMethods.Modulus,      args => GetValue(args[0]) % GetValue(args[1]) },
                { (int)DefaultMethods.Power,        args => Math.Pow(GetValue(args[0]), GetValue(args[1])) },

                { (int)DefaultMethods.EqualTo,      args => BoolToNum(GetValue(args[0]) == GetValue(args[1])) },
                { (int)DefaultMethods.NotEqualTo,   args => BoolToNum(GetValue(args[0]) != GetValue(args[1])) },

                { (int)DefaultMethods.GreaterThan,  args => BoolToNum(GetValue(args[0]) > GetValue(args[1])) },
              { (int)DefaultMethods.GreaterOrEqual, args => BoolToNum(GetValue(args[0]) >= GetValue(args[1])) },
                { (int)DefaultMethods.LessThan,     args => BoolToNum(GetValue(args[0]) < GetValue(args[1])) },
                { (int)DefaultMethods.LessOrEqual,  args => BoolToNum(GetValue(args[0]) <= GetValue(args[1])) },

                { (int)DefaultMethods.Not,          args => BoolToNum(GetValue(args[0]) == 0) },
                { (int)DefaultMethods.And,          args => BoolToNum(NumToBool(GetValue(args[0])) && NumToBool(GetValue(args[1]))) },
                { (int)DefaultMethods.Or,           args => BoolToNum(NumToBool(GetValue(args[0])) || NumToBool(GetValue(args[1]))) },
                { (int)DefaultMethods.XOr,          args => BoolToNum(NumToBool(GetValue(args[0])) ^ NumToBool(GetValue(args[1]))) },

                { (int)DefaultMethods.Print,        args => { Print(args); return 1; } },
                { (int)DefaultMethods.Read,         args => ReadLine() },
                { (int)DefaultMethods.GetInput,     args => GetInputChar() },

                { (int)DefaultMethods.If, args =>
                    {
                        if (NumToBool(GetValue(args[0]))) Evaluate(args[1]);
                        return 0;
                    }
                },
                { (int)DefaultMethods.IfElse, args =>
                    {
                        if (NumToBool(GetValue(args[0]))) Evaluate(args[1]);
                        else Evaluate(args[2]);
                        return 0;
                    }
                },
                { (int)DefaultMethods.While, args =>
                    {
                        while (NumToBool(GetValue(args[0]))) Evaluate(args[1]);
                        return 0;
                    }
                },

            };

        }

        private double GetInputChar()
        {
            try
            {
                return InputQueue.Dequeue();
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
        }
        private void Print(Command[] args)
        {
            DidPrintText = true;
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
            return val != 0;
        }
        private static int BoolToNum(bool bl)
        {
            return bl ? 1 : 0;
        }
        private double GetValue(Command cmd)
        {
            int value = cmd.Value ?? 0;
            return cmd.Type switch
            {
                CommandType.Literal => value,

                CommandType.Identifier => NumVariables[value],

                CommandType.Function => Functions[value].Invoke(cmd.Arguments),

                CommandType.Array =>
                    throw new ArgumentException($"Line {cmd.Line}: Expected single value, got array"),

                _ => 0, // unreachable
            }
            ;
        }
        private static int GetIdentifier(Command cmd)
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
                foreach (Command arg in cmd.Arguments) sb.Append(GetString(arg));
                return sb.ToString();
            }
            else return $"{(char)GetValue(cmd)}";
        }

        private double DeclareVar(int name, double value = 0)
        {
            NumVariables.Add(name, value);
            return value;
        }
        private double AssignVar(int name, double value)
        {
            NumVariables[name] = value;
            return value;
        }
        private double DeleteVar(int name)
        {
            double output = NumVariables[name];
            NumVariables.Remove(name);
            return output;
        }

        public Argument Evaluate(Command cmd, bool endStr = false)
        {
            if (endStr) DidPrintText = false;
            Argument output = new();

            switch (cmd.Type)
            {
                case CommandType.Array:
                    List<Argument> outputList = [];
                    Array.ForEach(cmd.Arguments, arg => outputList.Add(Evaluate(arg)));
                    output = new ArgumentArray([.. outputList]);
                    break;

                case CommandType.Literal:
                    output = new ArgumentDouble(cmd.Value ?? 0);
                    break;

                case CommandType.Function:
                    output = new ArgumentDouble(Functions[cmd.Value ?? 0].Invoke(cmd.Arguments));
                    break;

                case CommandType.Identifier:
                    output = new ArgumentDouble(NumVariables[cmd.Value ?? 0]);
                    break;

                default: break; // unreachable
            }

            if (DidPrintText && endStr) Console.WriteLine();
            return output;
        }

        public void Execute(Command[] cmds)
        {
            foreach (var cmd in cmds)
            {
                Evaluate(cmd);
            }
        }
    }
}
