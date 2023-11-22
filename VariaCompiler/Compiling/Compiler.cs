using VariaCompiler.Parsing.Nodes;

namespace VariaCompiler.Compiling;

public class Compiler : IAstVisitor
{
    private List<Instruction> intermediateCode = new List<Instruction>();
    private Dictionary<string, long> variableMemoryMap = new Dictionary<string, long>();
    private long nextMemoryAddress = 0;

    private void Emit(Instruction.OpCode op, string src = null, string dest = null)
    {
        intermediateCode.Add(new Instruction { Operation = op, Source = src, Destination = dest });
    }

    public List<string> Compile(AstNode node)
    {
        intermediateCode.Clear();
        node.Accept(this);
        OptimizeIntermediateCode();
        return GenerateMachineCode();
    }

    private void OptimizeIntermediateCode() {
        for (int i = 0; i < intermediateCode.Count - 1; i++) {
            var currentInstr = intermediateCode[i];
            var nextInstr = intermediateCode[i + 1];

            if (currentInstr.Operation == Instruction.OpCode.Mov && nextInstr.Operation == Instruction.OpCode.Mov &&
                currentInstr.Source == nextInstr.Destination && currentInstr.Destination == nextInstr.Source) {
                intermediateCode.RemoveAt(i + 1);
                i--;
                continue;
            }

            if (currentInstr.Operation == Instruction.OpCode.Push && nextInstr.Operation == Instruction.OpCode.Pop) {
                if (i + 2 < intermediateCode.Count) {
                    var addInstr = intermediateCode[i + 2];
                    if (addInstr.Operation == Instruction.OpCode.Add && addInstr.Source == "rb")
                    {
                        addInstr.Source = currentInstr.Source;
                        intermediateCode.RemoveAt(i);
                        intermediateCode.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }

    private List<string> GenerateMachineCode()
    {
        List<string> machineCode = new List<string>();
        foreach (var instr in intermediateCode)
        {
            switch (instr.Operation)
            {
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
                case Instruction.OpCode.Ret:
                    machineCode.Add("ret");
                    break;
            }
        }

        return machineCode;
    }

    public void Visit(FunctionDeclarationNode node)
    {
        foreach (var statement in node.Body)
        {
            statement.Accept(this);
        }
    }

    public void Visit(VariableDeclarationNode node)
    {
        node.Value.Accept(this);
        if (!variableMemoryMap.ContainsKey(node.Name))
        {
            variableMemoryMap[node.Name] = nextMemoryAddress;
            nextMemoryAddress += 8;
        }

        Emit(Instruction.OpCode.Mov, "ra", $"[{variableMemoryMap[node.Name]}]");
    }

    public void Visit(BinaryOperationNode node)
    {
        node.Left.Accept(this);
        Emit(Instruction.OpCode.Push, "ra");
        node.Right.Accept(this);
        Emit(Instruction.OpCode.Pop, null, "rb");
        if (node.Operator.Value == "+")
        {
            Emit(Instruction.OpCode.Add, "rb", "ra");
        }
    }

    public void Visit(LiteralNode node)
    {
        Emit(Instruction.OpCode.Mov, node.Value.Value.ToString(), "ra");
    }

    public void Visit(ReturnNode node)
    {
        node.Value.Accept(this);
        Emit(Instruction.OpCode.Mov, "ra", "[rsp]");
        Emit(Instruction.OpCode.Ret);
    }

    public void Visit(IdentifierNode node)
    {
        if (!variableMemoryMap.ContainsKey(node.Value.Value))
        {
            throw new Exception($"Undefined variable: {node.Value.Value}");
        }

        Emit(Instruction.OpCode.Mov, $"[{variableMemoryMap[node.Value.Value]}]", "ra");
    }
}