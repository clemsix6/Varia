using BlueCodeInterpreter;
using VariaCompiler.Compiling;
using VariaCompiler.Lexer;
using VariaCompiler.Parsing;
using VariaCompiler.Parsing.Nodes;

var lexer = new Lexer();
var testContent = File.ReadAllText("test.vr");
var tokens = lexer.Tokenize(testContent);
Console.WriteLine(string.Join("\n", tokens) + "\n\n");

var parser = new Parser(tokens);
var ast = parser.Parse();
PrintAst(ast, 0);
Console.WriteLine("\n");


var compiler = new Compiler();
var blueCodeInstructions = compiler.Compile(ast);
var blueCode = string.Join("\n", blueCodeInstructions);
Console.WriteLine(blueCode);

var instructions = File.ReadAllLines("test.bc").ToList();
var vm = new VirtualMachine();
vm.Execute(instructions);


static void PrintAst(AstNode node, int level) {
    var indent = new string(' ', level * 4);
    Console.WriteLine(indent + node);

    if (node is FunctionDeclarationNode funcNode) {
        foreach (var child in funcNode.Body) {
            PrintAst(child, level + 1);
        }
    } else if (node is VariableDeclarationNode varNode) {
        PrintAst(varNode.Value, level + 1);
    } else if (node is BinaryOperationNode binNode) {
        PrintAst(binNode.Left, level + 1);
        PrintAst(binNode.Right, level + 1);
    } else if (node is ReturnNode returnNode) {
        PrintAst(returnNode.Value, level + 1);
    }
}