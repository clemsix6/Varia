using VariaCompiler.Parsing.Nodes;


namespace VariaCompiler.Compiling;


public class Compiler : IAstVisitor
{
    private readonly List<Instruction> intermediateCode = new();
    private readonly Dictionary<string, long> variableMemoryMap = new();
    private long nextMemoryAddress = 0;
    private long pointIndex = 0;


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
        return GenerateMachineCode();
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
                case Instruction.OpCode.JmpIfNot:
                    machineCode.Add($"jne\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.Label:
                    machineCode.Add($"pt\t{instr.Source}");
                    break;
                case Instruction.OpCode.Jmp:
                    machineCode.Add($"jmp\t{instr.Source}");
                    break;
                case Instruction.OpCode.CmpEq:
                    machineCode.Add($"cmpeq\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.CmpNe:
                    machineCode.Add($"cmpne\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.CmpLt:
                    machineCode.Add($"cmplt\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.CmpGt:
                    machineCode.Add($"cmpgt\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.CmpLe:
                    machineCode.Add($"cmple\t{instr.Source} {instr.Destination}");
                    break;
                case Instruction.OpCode.CmpGe:
                    machineCode.Add($"cmpge\t{instr.Source} {instr.Destination}");
                    break;
            }
        }

        return machineCode;
    }


    public void Visit(FunctionDeclarationNode node)
    {
        this.variableMemoryMap.Clear();
        this.nextMemoryAddress = 0;
        this.pointIndex = 0;
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
        if (node.Operator.Value == "=") {
            node.Right.Accept(this);
            if (!this.variableMemoryMap.TryGetValue(
                    ((IdentifierNode)node.Left).Value.Value, out var address
                ))
                throw new Exception(
                    $"Undefined variable: {((IdentifierNode)node.Left).Value.Value}"
                );
            Emit(Instruction.OpCode.Mov, "ra", $"[{address}]");
            return;
        }

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
        Emit(Instruction.OpCode.Push, "ra");
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


    public void Visit(ConditionNode node)
    {
        node.Condition.Accept(this);
        var endIfLabel = $"PT_{this.pointIndex++}";
        Emit(Instruction.OpCode.JmpIfNot, "ra", endIfLabel);
        foreach (var statement in node.ThenBranch)
            statement.Accept(this);
        if (node.ElseBranch != null && node.ElseBranch.Any()) {
            var elseLabel = $"PT_{this.pointIndex++}";
            Emit(Instruction.OpCode.Jmp, elseLabel);
            Emit(Instruction.OpCode.Label, endIfLabel);
            foreach (var statement in node.ElseBranch)
                statement.Accept(this);
            Emit(Instruction.OpCode.Label, elseLabel);
        } else {
            Emit(Instruction.OpCode.Label, endIfLabel);
        }
    }


    public void Visit(ConditionalOperationNode node)
    {
        node.Left.Accept(this);
        Emit(Instruction.OpCode.Push, "ra");
        node.Right.Accept(this);
        Emit(Instruction.OpCode.Pop, null, "rb");

        switch (node.Operator.Value) {
            case "==":
                Emit(Instruction.OpCode.CmpEq, "rb", "ra");
                break;
            case "!=":
                Emit(Instruction.OpCode.CmpNe, "rb", "ra");
                break;
            case "<":
                Emit(Instruction.OpCode.CmpLt, "rb", "ra");
                break;
            case ">":
                Emit(Instruction.OpCode.CmpGt, "rb", "ra");
                break;
            case "<=":
                Emit(Instruction.OpCode.CmpLe, "rb", "ra");
                break;
            case ">=":
                Emit(Instruction.OpCode.CmpGe, "rb", "ra");
                break;
        }
    }


    public void Visit(WhileNode node)
    {
        var loopStartLabel = $"LOOP_START_{this.pointIndex++}";
        var loopEndLabel = $"LOOP_END_{this.pointIndex++}";

        Emit(Instruction.OpCode.Label, loopStartLabel);
        node.Condition.Accept(this);
        Emit(Instruction.OpCode.JmpIfNot, "ra", loopEndLabel);
        foreach (var statement in node.Body)
            statement.Accept(this);
        Emit(Instruction.OpCode.Jmp, loopStartLabel);
        Emit(Instruction.OpCode.Label, loopEndLabel);
    }


    public void Visit(BreakNode node)
    {
        Emit(Instruction.OpCode.Jmp, $"LOOP_END_{this.pointIndex - 2}");
    }
}