using BackshotsEsolang;
using BackshotsEsolang.Interpreting;
using BackshotsEsolang.Lexing;
using BackshotsEsolang.Parsing;

class Program
{
    static string GetFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException();

        return File.ReadAllText(path);
    }
    static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                Interpreter interpreter = new();

                while (true)
                {
                    Console.Write(">>");
                    string input = Console.ReadLine() ?? "";
                    if (input == "exit") break;
                    if (input == "file") 
                    {
                        Console.Write(">>input file:");
                        input = GetFile(Console.ReadLine() ?? "");
                    }

                    var tokenized = Lexer.Tokenize(input);
                    var parsed = Parser.Parse(tokenized);
                    foreach (var cmd in parsed)
                    {
                        Utils.PrintColored(interpreter.Evaluate(cmd).ToString(), ConsoleColor.Cyan);
                    }
                }
            }
            else
            {
                string code = GetFile(args[0]);
                var tokenized = Lexer.Tokenize(code);
                var parsed = Parser.Parse(tokenized);
                new Interpreter().Execute(parsed);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            return 1;
        }
        return 0;
    }
}