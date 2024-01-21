using VariaCompiler.Lexing;
using VariaCompiler.Parsing.Nodes;


namespace VariaCompiler.Parsing;


public class Parser
{
    private List<Token> tokens;
    private int position;


    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.position = 0;
    }


    private Token CurrentToken =>
        (this.position < this.tokens.Count ? this.tokens[this.position] : null) ??
        throw new
            InvalidOperationException();


    private Token ConsumeToken()
    {
        return this.tokens[this.position++];
    }


    private bool Match(TokenType type)
    {
        return this.CurrentToken.Type == type;
    }


    private void Expect(TokenType type, string errorMessage)
    {
        if (!Match(type)) {
            throw new Exception(errorMessage);
        }
        ConsumeToken();
    }


    private FunctionDeclarationNode ParseFunction()
    {
        Expect(TokenType.Keyword, "Expected 'func'");
        var functionName = this.CurrentToken.Value;
        Expect(TokenType.Identifier, "Expected function name");
        Expect(TokenType.Punctuation, "Expected '('");
        Expect(TokenType.Punctuation, "Expected ')'");
        Expect(TokenType.Punctuation, "Expected '{'");

        var body = new List<AstNode>();
        while (this.position < this.tokens.Count &&
               !(Match(TokenType.Punctuation) && this.CurrentToken.Value == "}")) {
            if (Match(TokenType.Keyword) && this.CurrentToken.Value == "var") {
                body.Add(ParseVariableDeclaration());
            } else if (Match(TokenType.Keyword) && this.CurrentToken.Value == "return") {
                body.Add(ParseReturn());
            } else if (Match(TokenType.Identifier)) {
                var lookahead = PeekNextToken();
                if (lookahead.Type == TokenType.Punctuation && lookahead.Value == "(") {
                    body.Add(ParseFunctionCall());
                } else {
                    ConsumeToken();
                }
            } else {
                ConsumeToken();
            }
        }
        Expect(TokenType.Punctuation, "Expected '}'");
        return new FunctionDeclarationNode
        {
            Name = functionName,
            Body = body
        };
    }


    private FunctionCallNode ParseFunctionCall()
    {
        var functionName = ConsumeToken();
        Expect(TokenType.Punctuation, "Expected '('");
        var arguments = new List<AstNode>();
        while (!Match(TokenType.Punctuation) ||
               (Match(TokenType.Punctuation) && CurrentToken.Value != ")")) {
            arguments.Add(ParseExpression());
            if (Match(TokenType.Punctuation) && CurrentToken.Value == ",") {
                ConsumeToken();
            }
        }
        Expect(TokenType.Punctuation, "Expected ')'");
        return new FunctionCallNode
        {
            FunctionName = functionName.Value,
            Arguments = arguments
        };
    }


    private VariableDeclarationNode ParseVariableDeclaration()
    {
        Expect(TokenType.Keyword, "Expected 'var'");
        var varName = this.CurrentToken.Value;
        Expect(TokenType.Identifier, "Expected variable name");
        Expect(TokenType.Operator, "Expected '='");
        var value = ParseExpression();

        return new VariableDeclarationNode
        {
            Name = varName,
            Value = value
        };
    }


    private ReturnNode ParseReturn()
    {
        Expect(TokenType.Keyword, "Expected 'return'");
        var value = ParseExpression();

        return new ReturnNode { Value = value };
    }


    private AstNode ParseFactor()
    {
        if (Match(TokenType.Identifier)) {
            var lookahead = PeekNextToken();
            if (lookahead.Type == TokenType.Punctuation && lookahead.Value == "(") {
                return ParseFunctionCall();
            }
            var token = ConsumeToken();
            return new IdentifierNode { Value = token };
        }
        if (Match(TokenType.LiteralNumber)) {
            var token = ConsumeToken();
            return new LiteralNode { Value = token };
        }
        if (Match(TokenType.LiteralString)) {
            var token = ConsumeToken();
            return new LiteralNode { Value = token };
        }
        if (Match(TokenType.Punctuation) && this.CurrentToken.Value == "(") {
            ConsumeToken();
            var expr = ParseExpression();
            Expect(TokenType.Punctuation, "Expected ')'");
            return expr;
        }
        throw new Exception("Unexpected token");
    }


    private AstNode ParseTerm()
    {
        var left = ParseFactor();

        while (Match(TokenType.Operator) && this.CurrentToken.Value is "*" or "/") {
            var op = ConsumeToken();
            var right = ParseFactor();

            left = new BinaryOperationNode
            {
                Left = left,
                Operator = op,
                Right = right
            };
        }

        return left;
    }


    private AstNode ParseExpression()
    {
        var left = ParseTerm();

        while (Match(TokenType.Operator) && this.CurrentToken.Value is "+" or "-") {
            var op = ConsumeToken();
            var right = ParseTerm();

            left = new BinaryOperationNode
            {
                Left = left,
                Operator = op,
                Right = right
            };
        }

        return left;
    }


    private Token PeekNextToken()
    {
        return ((this.position + 1 < this.tokens.Count) ? this.tokens[this.position + 1] : null) ??
               throw new
                   InvalidOperationException();
    }


    public ProgramNode Parse()
    {
        var programNode = new ProgramNode();
        while (this.position < this.tokens.Count) {
            if (this.CurrentToken.Type == TokenType.Keyword && this.CurrentToken.Value == "func") {
                var functionNode = ParseFunction();
                programNode.Functions.Add(functionNode);
            } else {
                ConsumeToken();
            }
        }
        return programNode;
    }
}