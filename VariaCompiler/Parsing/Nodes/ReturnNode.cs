using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class ReturnNode : AstNode
{
    public AstNode Value { get; init; } = null!;


    public override string ToString()
    {
        return $"ReturnNode ({this.Value})";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}