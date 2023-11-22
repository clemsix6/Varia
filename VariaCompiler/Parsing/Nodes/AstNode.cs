using VariaCompiler.Compiling;

namespace VariaCompiler.Parsing.Nodes;

public abstract class AstNode
{
    public abstract void Accept(IAstVisitor visitor);
}
