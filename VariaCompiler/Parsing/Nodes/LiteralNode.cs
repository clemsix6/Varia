using VariaCompiler.Compiling;
using VariaCompiler.Lexing;


namespace VariaCompiler.Parsing.Nodes;


public class LiteralNode : AstNode
{
    public Token Value { get; init; } = null!;


    public override string ToString()
    {
        return $"LiteralNode ({this.Value})";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}