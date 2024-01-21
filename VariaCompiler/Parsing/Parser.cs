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
            body.Add(ParseStatement());
        }
        Expect(TokenType.Punctuation, "Expected '}'");

        return new FunctionDeclarationNode
        {
            Name = functionName,
            Body = body
        };
    }


    private AstNode ParseStatement()
    {
        if (Match(TokenType.Keyword)) {
            var keyword = this.CurrentToken.Value;
            switch (keyword) {
                case "var":
                    return ParseVariableDeclaration();
                case "return":
                    return ParseReturn();
                case "if":
                    return ParseIf();
                case "while":
                    return ParseWhile();
                case "break":
                    return ParseBreak();
            }
        }
        if (Match(TokenType.Identifier)) {
            var lookahead = PeekNextToken();
            if (lookahead.Type == TokenType.Punctuation && lookahead.Value == "(")
                return ParseFunctionCall();
            return ParseExpression();
        }
        throw new Exception($"Unexpected token: \"{this.CurrentToken.Value}\"");
    }


    private WhileNode ParseWhile()
    {
        Expect(TokenType.Keyword, "Expected 'while'");
        var condition = ParseExpression();
        Expect(TokenType.Punctuation, "Expected '{'");

        var body = new List<AstNode>();
        while (!(Match(TokenType.Punctuation) && this.CurrentToken.Value == "}")) {
            body.Add(ParseStatement());
        }
        Expect(TokenType.Punctuation, "Expected '}'");

        return new WhileNode
        {
            Condition = condition,
            Body = body
        };
    }


    private BreakNode ParseBreak()
    {
        Expect(TokenType.Keyword, "Expected 'break'");
        return new BreakNode();
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

        while (true) {
            if (Match(TokenType.Operator) &&
                this.CurrentToken.Value.Length == 1 &&
                this.CurrentToken.Value != "=") {
                var op = ConsumeToken();
                var right = ParseTerm();
                left = new BinaryOperationNode { Left = left, Operator = op, Right = right };
            } else if (Match(TokenType.ConditionalOperator)) {
                var op = ConsumeToken();
                var right = ParseTerm();
                left = new ConditionalOperationNode { Left = left, Operator = op, Right = right };
            } else if (Match(TokenType.Operator) && this.CurrentToken.Value == "=") {
                var op = ConsumeToken();
                var right = ParseExpression();
                left = new BinaryOperationNode { Left = left, Operator = op, Right = right };
            } else {
                break;
            }
        }
        return left;
    }


    private ConditionNode ParseIf()
    {
        Expect(TokenType.Keyword, "Expected 'if'");
        var condition = ParseExpression();
        Expect(TokenType.Punctuation, "Expected '{'");

        var thenBranch = new List<AstNode>();
        while (!(Match(TokenType.Punctuation) && this.CurrentToken.Value == "}")) {
            thenBranch.Add(ParseStatement());
        }
        Expect(TokenType.Punctuation, "Expected '}'");

        List<AstNode> elseBranch = null;
        if (Match(TokenType.Keyword) && this.CurrentToken.Value == "else") {
            ConsumeToken();
            Expect(TokenType.Punctuation, "Expected '{'");
            elseBranch = new List<AstNode>();
            while (!(Match(TokenType.Punctuation) && this.CurrentToken.Value == "}")) {
                elseBranch.Add(ParseStatement());
            }
            Expect(TokenType.Punctuation, "Expected '}'");
        }

        return new ConditionNode
        {
            Condition = condition,
            ThenBranch = thenBranch,
            ElseBranch = elseBranch
        };
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