using VariaCompiler.Compiling;
using VariaCompiler.Lexer;

namespace VariaCompiler.Parsing.Nodes;

public class LiteralNode : AstNode {
    public Token Value { get; set; }
    
    
    public override string ToString()
    {
        return $"LiteralNode ({Value})";
    }
    
    
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}