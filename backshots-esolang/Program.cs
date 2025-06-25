using BackshotsEsolang.Interpreting;
using BackshotsEsolang.Lexing;
using BackshotsEsolang.Parsing;

class Program
{
    static void PrintColored(string text, ConsoleColor color)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = originalColor;
    }
    static void LogArray<T>(T[] array)
    {
        foreach (T item in array)
        {
            if (item == null) continue;
            Console.WriteLine(item.ToString() + ",");
        }
    }
    static int End(int value)
    {
        Console.Write("\nPress any key to close...");
        Console.Read();
        return value;
    }
    static string GetFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException();

        return File.ReadAllText(path);
    }
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Interpreter interpreter = new();

            while (true)
            {
                try
                {
                    Console.Write(">>");
                    string input = Console.ReadLine() ?? "";
                    if (input == "exit") break;
                    if (input == "test") input = GetFile(@"C:\Users\alunos\Documents\programar\vs\backshots-esolang\backshots-esolang\test.bs");
                    if (input == "file")
                    {
                        Console.Write(">>input file:");
                        input = GetFile(Console.ReadLine() ?? "");
                    }
                    if (input == "reset")
                    {
                        interpreter = new Interpreter();
                        Console.WriteLine("Interpreter Reset.");
                        continue;
                    }

                    var tokenized = Lexer.Tokenize(input);
                    var parsed = Parser.Parse(tokenized);
                    foreach (var cmd in parsed)
                    {
                        var evaluated = interpreter.Evaluate(cmd, true);
                        PrintColored(evaluated.ToString(), ConsoleColor.Cyan);
                    }
                }
                catch (Exception e)
                {
                    PrintColored("Error: " + e.Message, ConsoleColor.Red);
                }
            }
        }
        else
        {
            try
            {
                string code = GetFile(args[0]);
                var tokenized = Lexer.Tokenize(code);
                var parsed = Parser.Parse(tokenized);
                new Interpreter().Execute(parsed);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return End(1);
            }
        }
        return End(0);
    }
}