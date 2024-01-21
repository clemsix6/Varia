using System.Text.RegularExpressions;


namespace VariaCompiler.Lexing;


public class Lexer
{
    private readonly Dictionary<TokenType, Regex> patterns = new()
    {
        { TokenType.Keyword, new Regex(@"\b(func|var|return|if|else)\b") },
        { TokenType.Identifier, new Regex(@"[a-zA-Z_][a-zA-Z_0-9]*") },
        { TokenType.ConditionalOperator, new Regex(@"==|<=|>=|!=|<|>") },
        { TokenType.Operator, new Regex(@"=|\+|\*") },
        { TokenType.LiteralNumber, new Regex(@"\d+(\.\d+)?") },
        { TokenType.Punctuation, new Regex(@"\(|\)|\{|\}|;") },
        { TokenType.FunctionCall, new Regex(@"[a-zA-Z_][a-zA-Z_0-9]*\s*\(") },
        { TokenType.LiteralString, new Regex("\".*?\"") }
    };


    public List<Token> Tokenize(string source)
    {
        var tokens = new List<Token>();

        while (!string.IsNullOrEmpty(source)) {
            var matched = false;
            foreach (var pattern in this.patterns) {
                var match = pattern.Value.Match(source);
                if (match.Success && match.Index == 0) {
                    tokens.Add(new Token(pattern.Key, match.Value));
                    source = source[match.Length..].Trim();
                    matched = true;
                    break;
                }
            }
            if (!matched)
                throw new Exception($"Invalid token in source: {source}");
        }
        return tokens;
    }
}