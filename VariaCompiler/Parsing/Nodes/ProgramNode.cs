using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class ProgramNode : AstNode
{
    public List<FunctionDeclarationNode> Functions { get; } = new();


    public override string ToString()
    {
        return $"ProgramNode ({this.Functions.Count} functions)";
    }


    public override void Accept(IAstVisitor visitor)
    {
    }
}