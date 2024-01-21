using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class FunctionCallNode : AstNode
{
    public string FunctionName { get; init; } = null!;
    public List<AstNode> Arguments { get; init; } = null!;


    public override string ToString()
    {
        var args = string.Join(", ", this.Arguments.Select(arg => arg.ToString()));
        return $"FunctionCallNode ({this.FunctionName}({args}))";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}