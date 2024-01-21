using VariaCompiler.Parsing.Nodes;

namespace VariaCompiler.Compiling;

public interface IAstVisitor {
    void Visit(FunctionDeclarationNode node);
    void Visit(VariableDeclarationNode node);
    void Visit(BinaryOperationNode node);
    void Visit(LiteralNode node);
    void Visit(ReturnNode node);
    void Visit(IdentifierNode node);
    void Visit(FunctionCallNode node);
    void Visit(ConditionNode node);
    void Visit(ConditionalOperationNode node);
}