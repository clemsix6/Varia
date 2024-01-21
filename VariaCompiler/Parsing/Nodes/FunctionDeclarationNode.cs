using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class FunctionDeclarationNode : AstNode
{
    public string Name { get; init; } = null!;
    public List<AstNode> Body { get; init; }= null!;


    public override string ToString()
    {
        return $"FunctionDeclarationNode ({this.Name})";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}