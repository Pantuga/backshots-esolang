using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BackshotsEsolang.Lexing;

namespace BackshotsEsolang.Parsing
{
    public class Parser(Token[] tokens)
    {
        private readonly Token[] _tokens = tokens;
        private int _index = 0;

        public static Command[] Parse(Token[] tokens)
        {
            return new Parser(tokens).Parsed();
        }

        public static int ClusterToInt(Cluster cluster)
        {
            return (int)((cluster.Quantity - 1)  + (10 * ((int)cluster.Character - 1)));
        }
        public static int ClusterArrayToInt(Cluster[] clusters)
        {
            int output = 0;
            for (int i = 0; i < clusters.Length; i++) 
            {
                Cluster cl = clusters[i];
                output += (int)(cl.Quantity * Math.Pow(10, ((int)cl.Character - 1)));
            }
            return output;
        }
        private static int GetTokenValue(Token token, int index)
        {
            return ClusterArrayToInt([.. token.Clusters[index..]]);
        }

        public Command[] Parsed()
        {
            _index = 0;
            List<Command> commands = [];

            while (_index < _tokens.Length)
            {
                commands.Add(ParseCommand());
            }

            return [..commands];
        }

        private Command ParseCommand()
        {
            if (_index >= _tokens.Length)
                throw new Exception($"Line {_tokens[^1].line}: Unexpected end of input");

            Token currentToken = _tokens[_index];
            _index++;

            if (currentToken.Clusters.Count == 0)
                throw new Exception($"Line {currentToken.line}: Token has no clusters");

            // Handle single-cluster tokens as grouping markers
            if (currentToken.Clusters.Count == 1)
            {
                Cluster openingCluster = currentToken.Clusters[0];
                List<Token> groupedTokens = [];

                while (_index < _tokens.Length)
                {
                    Token nextToken = _tokens[_index];

                    if (nextToken.Clusters.Count == 1 &&
                        nextToken.Clusters[0].Is(openingCluster))
                    {
                        _index++;
                        break;
                    }

                    groupedTokens.Add(nextToken);
                    _index++;
                }

                // Parse the grouped tokens as arguments
                return new Command(currentToken.line, Parse([..groupedTokens]), CommandType.Array);
            }

            int argCount = ClusterToInt(currentToken.Clusters[0]);
            Command[] args = new Command[argCount];

            for (int i = 0; i < argCount; i++)
            {
                if (_index >= _tokens.Length)
                    throw new Exception($"Line {currentToken.line}: Expected {argCount} arguments, got {i}");

                args[i] = ParseCommand(); // Recursively parse arguments
            }

            CommandType commandType;
            int? value;
            if (argCount != 0) 
            {
                value = GetTokenValue(currentToken, 1);
                commandType = CommandType.Function;
            }
            else
            {
                if(currentToken.Clusters[1].Is(AllowedChar.G, 1))
                {
                    value = GetTokenValue(currentToken, 2);
                    commandType = CommandType.Literal;
                }
                else
                {
                    value = GetTokenValue(currentToken, 1);
                    commandType = CommandType.Identifier;
                }
            }
                /*
                CommandType cmdType = argCount == 0 ? (
                    currentToken.Clusters[1].Is(AllowedChar.G, 1) ?
                        CommandType.Literal :
                        CommandType.Identifier
                    ) :
                    CommandType.Function;
                */
            return new Command(currentToken.line, args, commandType, value);
        }
    }
}
