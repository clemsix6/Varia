using VariaCompiler.Compiling;
using VariaCompiler.Lexer;

namespace VariaCompiler.Parsing.Nodes;

public class IdentifierNode : AstNode
{
    public Token Value { get; set; }


    public override string ToString()
    {
        return $"IdentifierNode ({Value})";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}