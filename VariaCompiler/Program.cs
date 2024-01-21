﻿using VariaCompiler.Compiling;
using VariaCompiler.Interpreter;
using VariaCompiler.Lexing;
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

var vm = new VirtualMachine();
var exitCode = vm.Execute(blueCodeInstructions);


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

Console.WriteLine("\n\nExit code: " + exitCode + "\n\n");
