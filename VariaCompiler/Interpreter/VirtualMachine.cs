using System.Text.RegularExpressions;


namespace VariaCompiler.Interpreter;


public class VirtualMachine
{
    private Register ra, rb, rc, rd, re, rf, rg, rh, rsp;
    private List<Register> stack = new();
    private Dictionary<long, Register> memory = new();
    private Dictionary<long, Register> stackMemory = new();


    public VirtualMachine()
    {
        this.ra = new Register();
        this.rb = new Register();
        this.rc = new Register();
        this.rd = new Register();
        this.re = new Register();
        this.rf = new Register();
        this.rg = new Register();
        this.rh = new Register();
        this.rsp = new Register(0);
    }


    private readonly Regex movPattern = new(
        @"^\s*mov\s+([\w\d\[\].]+?)\s+([\w\d\[\].]+?)\s*$", RegexOptions.IgnoreCase
    );
    private readonly Regex addPattern = new(
        @"^\s*add\s+([\w\d\[\].]+?)\s+([\w\d\[\].]+?)\s*$", RegexOptions.IgnoreCase
    );
    private readonly Regex pushPattern = new(
        @"^\s*push\s+([\w\d\[\].]+?)\s*$", RegexOptions.IgnoreCase
    );
    private readonly Regex popPattern = new(@"^\s*pop\s+([\w\d\[\]]+?)\s*$", RegexOptions.IgnoreCase);
    private readonly Regex printPattern = new(
        @"^\s*print\s+([\w\d\[\].]+?)\s*$", RegexOptions.IgnoreCase
    );
    private readonly Regex retPattern = new(@"^\s*ret\s*$", RegexOptions.IgnoreCase);


    public string? Execute(List<string> blueCodeInstructions)
    {
        foreach (var instruction in blueCodeInstructions) {
            Match match;

            match = this.movPattern.Match(instruction);
            if (match.Success) {
                HandleMov(match.Groups[1].Value, match.Groups[2].Value);
                continue;
            }

            match = this.addPattern.Match(instruction);
            if (match.Success) {
                HandleAdd(match.Groups[1].Value, match.Groups[2].Value);
                continue;
            }

            match = this.pushPattern.Match(instruction);
            if (match.Success) {
                HandlePush(match.Groups[1].Value);
                continue;
            }

            match = this.popPattern.Match(instruction);
            if (match.Success) {
                HandlePop(match.Groups[1].Value);
                continue;
            }

            match = this.printPattern.Match(instruction);
            if (match.Success) {
                var value = GetValue(match.Groups[1].Value);
                Console.WriteLine(value);
                continue;
            }

            match = this.retPattern.Match(instruction);
            if (match.Success) {
                return this.ra.GetStringValue();
            }


            throw new Exception($"Instruction {instruction} not recognized.");
        }
        return this.ra.GetStringValue();
    }


    private void HandleMov(string src, string dest)
    {
        var value = GetValue(src);
        SetValue(dest, value);
    }


    private void HandleAdd(string src, string dest)
    {
        var value1 = GetValue(src);
        var value2 = GetValue(dest);
        SetValue(dest, value1.Add(value2));
    }


    private void HandlePush(string src)
    {
        var value = GetValue(src);
        this.stackMemory[this.rsp.GetIntValue()] = value;
        this.rsp.Add(1);
    }


    private void HandlePop(string dest)
    {
        this.rsp.Sub(1);
        var value = this.stackMemory[this.rsp.GetIntValue()];
        this.stackMemory.Remove(this.rsp.GetIntValue());
        SetValue(dest, value);
    }


    private Register GetValue(string operand)
    {
        if (operand == "rsp")
            return this.rsp.Clone();
        if (double.TryParse(operand, out var immediateValue))
            return new Register(immediateValue);
        if (operand.StartsWith("[") && operand.EndsWith("]")) {
            var address = long.Parse(operand.Trim('[', ']'));
            return this.memory[address].Clone();
        }

        return operand switch
        {
            "ra" => this.ra.Clone(),
            "rb" => this.rb.Clone(),
            "rc" => this.rc.Clone(),
            "rd" => this.rd.Clone(),
            "re" => this.re.Clone(),
            "rf" => this.rf.Clone(),
            "rg" => this.rg.Clone(),
            "rh" => this.rh.Clone(),
            "rsp" => this.rsp.Clone(),
            _ => throw new Exception($"Operand {operand} not recognized."),
        };
    }


    private void SetValue(string operand, Register value)
    {
        value = value.Clone();
        if (operand.StartsWith("[") && operand.EndsWith("]")) {
            operand = operand.Trim('[', ']');
            if (operand == "rsp") {
                this.stackMemory[this.rsp.GetIntValue()] = value;
                return;
            }
            var address = long.Parse(operand);
            this.memory[address] = value;
            return;
        }

        switch (operand) {
            case "ra":
                this.ra = value;
                break;
            case "rb":
                this.rb = value;
                break;
            case "rc":
                this.rc = value;
                break;
            case "rd":
                this.rd = value;
                break;
            case "re":
                this.re = value;
                break;
            case "rf":
                this.rf = value;
                break;
            case "rg":
                this.rg = value;
                break;
            case "rh":
                this.rh = value;
                break;
            case "rsp":
                this.rsp = value;
                break;
            default: throw new Exception($"Operand {operand} not recognized.");
        }
    }
}