using System.Text.RegularExpressions;

namespace VariaCompiler.Lexer;

public class Lexer {

    private readonly Dictionary<TokenType, Regex> _patterns;

    public Lexer() {
        _patterns = new Dictionary<TokenType, Regex> {
            { TokenType.Keyword, new Regex(@"\b(func|var|return)\b") },
            { TokenType.Identifier, new Regex(@"[a-zA-Z_][a-zA-Z_0-9]*") },
            { TokenType.Operator, new Regex(@"=|\+") },
            { TokenType.LiteralInt, new Regex(@"\d+") },
            { TokenType.Punctuation, new Regex(@"\(|\)|\{|\}|;") }
        };
    }

    public List<Token> Tokenize(string source) {
        var tokens = new List<Token>();

        while (!string.IsNullOrEmpty(source)) {
            bool matched = false;
            
            foreach (var pattern in _patterns) {
                var match = pattern.Value.Match(source);
                if (match.Success && match.Index == 0) {
                    tokens.Add(new Token(pattern.Key, match.Value));
                    source = source.Substring(match.Length).Trim();
                    matched = true;
                    break;
                }
            }

            if (!matched) {
                throw new Exception($"Invalid token in source: {source}");
            }
        }

        return tokens;
    }
}