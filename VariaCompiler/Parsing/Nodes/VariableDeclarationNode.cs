using VariaCompiler.Compiling;

namespace VariaCompiler.Parsing.Nodes;

public class VariableDeclarationNode : AstNode {
    public string Name { get; set; }
    public AstNode Value { get; set; }
    
    
    public override string ToString()
    {
        return $"VariableDeclarationNode ({Name})";
    }
    
    
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}