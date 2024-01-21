using VariaCompiler.Compiling;
using VariaCompiler.Lexing;


namespace VariaCompiler.Parsing.Nodes;

public class BinaryOperationNode : AstNode {
    public AstNode Left { get; set; }
    public Token Operator { get; set; }
    public AstNode Right { get; set; }


    public override string ToString()
    {
        return $"BinaryOperationNode ({Operator})";
    }
    
    
    public override void Accept(IAstVisitor visitor) {
        visitor.Visit(this);
    }
}