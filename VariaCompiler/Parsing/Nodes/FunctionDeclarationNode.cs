using VariaCompiler.Compiling;

namespace VariaCompiler.Parsing.Nodes;

public class FunctionDeclarationNode : AstNode {
    public string Name { get; set; }
    public List<AstNode> Body { get; set; }


    public override string ToString()
    {
        return $"FunctionDeclarationNode ({Name})";
    }
    
    
    public override void Accept(IAstVisitor visitor) {
        visitor.Visit(this);
    }
}