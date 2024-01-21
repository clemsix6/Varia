using VariaCompiler.Compiling;


namespace VariaCompiler.Parsing.Nodes;


public class BreakNode : AstNode
{
    public override string ToString()
    {
        return "BreakNode";
    }


    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}