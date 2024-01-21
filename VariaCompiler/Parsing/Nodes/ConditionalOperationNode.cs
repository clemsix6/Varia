using VariaCompiler.Compiling;
using VariaCompiler.Lexing;


namespace VariaCompiler.Parsing.Nodes;


public class ConditionalOperationNode : AstNode
{
    public AstNode Left { get; init; } = null!;
    public Token Operator { get; init; } = null!;
    public AstNode Right { get; init; } = null!;

    public override string ToString()
    {
        return $"ConditionalOperationNode ({this.Operator})";
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
