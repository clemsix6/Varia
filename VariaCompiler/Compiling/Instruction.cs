namespace VariaCompiler.Compiling;

public class Instruction
{
    public enum OpCode {
        Mov, Push, Pop, Add, Sub, Mul, Div, Ret, Call, Def
    }

    public OpCode Operation { get; set; }
    public string? Source { get; set; }
    public string? Destination { get; set; }
}