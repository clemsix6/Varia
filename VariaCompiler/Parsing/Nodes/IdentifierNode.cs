using VariaCompiler.Compiling;
using VariaCompiler.Lexing;


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