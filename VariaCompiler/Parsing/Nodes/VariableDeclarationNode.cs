using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class VariableDeclarationNode : AstNode
{
    public string Name { get; init; } = null!;
    public AstNode Value { get; init; } = null!;


    public override string ToString()
    {
        return $"VariableDeclarationNode ({this.Name})";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}