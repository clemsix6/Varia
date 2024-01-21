using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class ConditionNode : AstNode
{
    public AstNode Condition { get; init; } = null!;
    public List<AstNode> ThenBranch { get; init; } = null!;
    public List<AstNode>? ElseBranch { get; init; } = null!;


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}