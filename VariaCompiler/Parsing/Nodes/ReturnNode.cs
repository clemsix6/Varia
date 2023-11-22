using VariaCompiler.Compiling;

namespace VariaCompiler.Parsing.Nodes;

public class ReturnNode : AstNode {
    public AstNode Value { get; set; }
    
    
    public override string ToString()
    {
        return $"ReturnNode ({Value})";
    }
    
    
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}