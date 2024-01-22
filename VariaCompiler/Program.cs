using VariaCompiler.Compiling;
using VariaCompiler.Interpreter;
using VariaCompiler.Lexing;
using VariaCompiler.Parsing;
using VariaCompiler.Parsing.Nodes;



namespace VariaCompiler;


internal static class Program
{
    private static List<Token> Lex(string fileName)
    {
        var lexer = new Lexer();
        var testContent = File.ReadAllText(fileName);
        var tokens = lexer.Tokenize(testContent);
        return tokens;
    }


    private static void PrintLexed(List<Token> tokens)
    {
        foreach (var token in tokens)
            Console.WriteLine(token);
        Console.WriteLine("\n\n");
    }


    private static void PrintAst(AstNode node, int level)
    {
        var indent = new string(' ', level * 4);
        Console.WriteLine(indent + node);

        if (node is ProgramNode programNode) {
            foreach (var child in programNode.Functions)
                PrintAst(child, level + 1);
        } else if (node is FunctionDeclarationNode funcNode) {
            foreach (var child in funcNode.Body)
                PrintAst(child, level + 1);
        } else if (node is VariableDeclarationNode varNode) {
            PrintAst(varNode.Value, level + 1);
        } else if (node is BinaryOperationNode binNode) {
            PrintAst(binNode.Left, level + 1);
            PrintAst(binNode.Right, level + 1);
        } else if (node is ReturnNode returnNode) {
            PrintAst(returnNode.Value, level + 1);
        } else if (node is WhileNode whileNode) {
            PrintAst(whileNode.Condition, level + 1);
            foreach (var child in whileNode.Body) {
                PrintAst(child, level + 1);
            }
        }
    }


    private static ProgramNode Parse(List<Token> tokens)
    {
        var parser = new Parser(tokens);
        return parser.Parse();
    }


    private static List<string> Compile(ProgramNode node)
    {
        var compiler = new Compiler();
        var blueCodeInstructions = compiler.Compile(node);
        return blueCodeInstructions;
    }


    private static void PrintBlueCode(List<string> blueCode)
    {
        foreach (var instruction in blueCode)
            Console.WriteLine(instruction);
        Console.WriteLine("\n\n");
    }


    private static string? Execute(List<string> blueCode)
    {
        var vm = new VirtualMachine();
        var exitCode = vm.Execute(blueCode);
        return exitCode;
    }


    private static void Main()
    {
        var tokens = Lex("test_loop.vr");
        PrintLexed(tokens);

        var ast = Parse(tokens);
        PrintAst(ast, 0);
        Console.WriteLine("\n\n");

        var blueCode = Compile(ast);
        PrintBlueCode(blueCode);

        var exitCode = Execute(blueCode);
        Console.WriteLine("Exit code: " + exitCode);
    }
}