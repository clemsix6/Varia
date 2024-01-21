using VariaCompiler.Parsing.Nodes;


namespace VariaCompiler.Compiling;


public class Compiler : IAstVisitor
{
    private readonly List<Instruction> intermediateCode = new();
    private readonly Dictionary<string, long> variableMemoryMap = new();
    private long nextMemoryAddress = 0;


    private void Emit(Instruction.OpCode op, string? src = null, string? dest = null)
    {
        this.intermediateCode.Add(
            new Instruction { Operation = op, Source = src, Destination = dest }
        );
    }


    public List<string> Compile(ProgramNode programNode)
    {
        this.intermediateCode.Clear();
        foreach (var functionNode in programNode.Functions)
            functionNode.Accept(this);
        OptimizeIntermediateCode();
        return GenerateMachineCode();
    }


    private void OptimizeIntermediateCode()
    {
        for (var i = 0; i < this.intermediateCode.Count - 1; i++) {
            var currentInstr = this.intermediateCode[i];
            var nextInstr = this.intermediateCode[i + 1];

            if (currentInstr.Operation == Instruction.OpCode.Mov &&
                nextInstr.Operation == Instruction.OpCode.Mov &&
                currentInstr.Source == nextInstr.Destination &&
                currentInstr.Destination == nextInstr.Source) {
                this.intermediateCode.RemoveAt(i + 1);
                i--;
                continue;
            }

            if (currentInstr.Operation == Instruction.OpCode.Push &&
                nextInstr.Operation == Instruction.OpCode.Pop) {
                if (i + 2 < intermediateCode.Count) {
                    var addInstr = intermediateCode[i + 2];
                    if (addInstr.Operation == Instruction.OpCode.Add && addInstr.Source == "rb") {
                        addInstr.Source = currentInstr.Source;
                        this.intermediateCode.RemoveAt(i);
                        this.intermediateCode.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }


    private List<string> GenerateMachineCode()
    {
        var machineCode = new List<string>();
        foreach (var instr in intermediateCode) {
            switch (instr.Operation) {
                case Instruction.OpCode.Mov:
                    machineCode.Add($"mov\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.Push:
                    machineCode.Add($"push\t{instr.Source}");
                    break;
                case Instruction.OpCode.Pop:
                    machineCode.Add($"pop\t{instr.Destination}");
                    break;
                case Instruction.OpCode.Add:
                    machineCode.Add($"add\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.Sub:
                    machineCode.Add($"sub\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.Mul:
                    machineCode.Add($"mul\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.Div:
                    machineCode.Add($"div\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.Def:
                    machineCode.Add($"--{instr.Source}");
                    break;
                case Instruction.OpCode.Call:
                    machineCode.Add($"call\t{instr.Source}");
                    break;
                case Instruction.OpCode.Ret:
                    machineCode.Add("ret");
                    break;
            }
        }

        return machineCode;
    }


    public void Visit(FunctionDeclarationNode node)
    {
        this.variableMemoryMap.Clear();
        this.nextMemoryAddress = 0;
        Emit(Instruction.OpCode.Def, node.Name);
        foreach (var statement in node.Body)
            statement.Accept(this);
    }


    public void Visit(VariableDeclarationNode node)
    {
        node.Value.Accept(this);
        if (this.variableMemoryMap.TryAdd(node.Name, this.nextMemoryAddress))
            this.nextMemoryAddress++;
        Emit(Instruction.OpCode.Mov, "ra", $"[{this.variableMemoryMap[node.Name]}]");
    }


    public void Visit(BinaryOperationNode node)
    {
        node.Left.Accept(this);
        Emit(Instruction.OpCode.Push, "ra");
        node.Right.Accept(this);
        Emit(Instruction.OpCode.Pop, null, "rb");
        switch (node.Operator.Value) {
            case "+":
                Emit(Instruction.OpCode.Add, "rb", "ra");
                break;
            case "-":
                Emit(Instruction.OpCode.Sub, "rb", "ra");
                break;
            case "*":
                Emit(Instruction.OpCode.Mul, "rb", "ra");
                break;
            case "/":
                Emit(Instruction.OpCode.Div, "rb", "ra");
                break;
        }
    }


    public void Visit(LiteralNode node)
    {
        Emit(Instruction.OpCode.Mov, node.Value.Value, "ra");
    }


    public void Visit(ReturnNode node)
    {
        node.Value.Accept(this);
        Emit(Instruction.OpCode.Ret);
    }


    public void Visit(IdentifierNode node)
    {
        if (!this.variableMemoryMap.TryGetValue(node.Value.Value, out var value))
            throw new Exception($"Undefined variable: {node.Value.Value}");
        Emit(Instruction.OpCode.Mov, $"[{value}]", "ra");
    }


    public void Visit(FunctionCallNode node)
    {
        foreach (var arg in node.Arguments)
            arg.Accept(this);
        Emit(Instruction.OpCode.Call, node.FunctionName);
        Emit(Instruction.OpCode.Push, "ra");
    }
}