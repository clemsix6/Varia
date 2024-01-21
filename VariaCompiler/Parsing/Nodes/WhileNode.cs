using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class WhileNode : AstNode
{
    public AstNode Condition { get; init; } = null!;
    public List<AstNode> Body { get; init; } = null!;


    public override string ToString()
    {
        return "WhileNode";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}