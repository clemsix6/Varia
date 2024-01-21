using VariaCompiler.Lexing;
using VariaCompiler.Parsing.Nodes;

namespace VariaCompiler.Parsing;

public class Parser {
    private List<Token> tokens;
    private int position;

    public Parser(List<Token> tokens) {
        this.tokens = tokens;
        position = 0;
    }

    private Token CurrentToken => position < tokens.Count ? tokens[position] : null;

    private Token ConsumeToken() {
        return tokens[position++];
    }

    private bool Match(TokenType type) {
        return CurrentToken != null && CurrentToken.Type == type;
    }

    private void Expect(TokenType type, string errorMessage) {
        if (!Match(type)) {
            throw new Exception(errorMessage);
        }
        ConsumeToken();
    }

    public FunctionDeclarationNode ParseFunction() {
        Expect(TokenType.Keyword, "Expected 'func'");
        var functionName = CurrentToken.Value;
        Expect(TokenType.Identifier, "Expected function name");

        Expect(TokenType.Punctuation, "Expected '('");
        Expect(TokenType.Punctuation, "Expected ')'");

        Expect(TokenType.Punctuation, "Expected '{'");

        var body = new List<AstNode>();
        while (!Match(TokenType.Punctuation) || (Match(TokenType.Punctuation) && CurrentToken.Value != "}")) {
            if (Match(TokenType.Keyword) && CurrentToken.Value == "var") {
                body.Add(ParseVariableDeclaration());
            } else if (Match(TokenType.Keyword) && CurrentToken.Value == "return") {
                body.Add(ParseReturn());
            }
        }
        
        Expect(TokenType.Punctuation, "Expected '}'");

        return new FunctionDeclarationNode {
            Name = functionName,
            Body = body
        };
    }

    public VariableDeclarationNode ParseVariableDeclaration() {
        Expect(TokenType.Keyword, "Expected 'var'");
        var varName = CurrentToken.Value;
        Expect(TokenType.Identifier, "Expected variable name");
        Expect(TokenType.Operator, "Expected '='");
        var value = ParseExpression();

        return new VariableDeclarationNode {
            Name = varName,
            Value = value
        };
    }

    public ReturnNode ParseReturn() {
        Expect(TokenType.Keyword, "Expected 'return'");
        var value = ParseExpression();

        return new ReturnNode { Value = value };
    }

    public AstNode ParseExpression() {
        var left = ParseTerm();
        
        while (Match(TokenType.Operator)) {
            var op = ConsumeToken();
            var right = ParseTerm();

            left = new BinaryOperationNode {
                Left = left,
                Operator = op,
                Right = right
            };
        }

        return left;
    }

    public AstNode ParseTerm() {
        if (Match(TokenType.LiteralNumber)) {
            var token = ConsumeToken();
            return new LiteralNode { Value = token };
        } else if (Match(TokenType.Identifier)) {
            var token = ConsumeToken();
            return new IdentifierNode { Value = token };
        }
        
        throw new Exception("Unexpected token");
    }

    public AstNode Parse() {
        return ParseFunction();
    }
}
