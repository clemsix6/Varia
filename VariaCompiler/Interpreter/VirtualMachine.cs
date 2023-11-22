using System.Text.RegularExpressions;

namespace BlueCodeInterpreter;

public class VirtualMachine
{
    private long ra, rb, rc, rd, re, rf, rg, rh, rsp;
    private List<long> stack = new List<long>();
    private Dictionary<long, long> memory = new Dictionary<long, long>();
    private Dictionary<long, long> stackMemory = new Dictionary<long, long>();

    public VirtualMachine() {
        ra = rb = rc = rd = re = rf = rg = rh = rsp = 0;
    }

    private Regex movPattern = new Regex(@"^\s*mov\s+([\w\d\[\]]+?)\s+([\w\d\[\]]+?)\s*$", RegexOptions.IgnoreCase);
    private Regex addPattern = new Regex(@"^\s*add\s+([\w\d\[\]]+?)\s+([\w\d\[\]]+?)\s*$", RegexOptions.IgnoreCase);
    private Regex pushPattern = new Regex(@"^\s*push\s+([\w\d\[\]]+?)\s*$", RegexOptions.IgnoreCase);
    private Regex popPattern = new Regex(@"^\s*pop\s+([\w\d\[\]]+?)\s*$", RegexOptions.IgnoreCase);
    private Regex printPattern = new Regex(@"^\s*print\s+([\w\d\[\]]+?)\s*$", RegexOptions.IgnoreCase);
    private Regex retPattern = new Regex(@"^\s*ret\s*$", RegexOptions.IgnoreCase);



    public void Execute(List<string> blueCodeInstructions) {
        foreach (var instruction in blueCodeInstructions) {
            Match match;

            match = movPattern.Match(instruction);
            if (match.Success) {
                HandleMOV(match.Groups[1].Value, match.Groups[2].Value);
                continue;
            }

            match = addPattern.Match(instruction);
            if (match.Success) {
                HandleADD(match.Groups[1].Value, match.Groups[2].Value);
                continue;
            }
            
            match = pushPattern.Match(instruction);
            if (match.Success) {
                HandlePUSH(match.Groups[1].Value);
                continue;
            }
            
            match = popPattern.Match(instruction);
            if (match.Success) {
                HandlePOP(match.Groups[1].Value);
                continue;
            }
            
            match = printPattern.Match(instruction);
            if (match.Success) {
                var value = GetValue(match.Groups[1].Value);
                Console.WriteLine(value);
                continue;
            }
            
            match = retPattern.Match(instruction);
            if (match.Success) {
                return;
            }


            throw new Exception($"Instruction {instruction} not recognized.");
        }
    }

    private void HandleMOV(string src, string dest) {
        var value = GetValue(src);
        SetValue(dest, value);
    }

    private void HandleADD(string src, string dest) {
        var value1 = GetValue(src);
        var value2 = GetValue(dest);
        SetValue(dest, value1 + value2);
    }

    private void HandlePUSH(string src) {
        var value = GetValue(src);
        stackMemory[rsp] = value;
        rsp += 8;
    }

    private void HandlePOP(string dest) {
        rsp -= 8;
        var value = stackMemory[rsp];
        stackMemory.Remove(rsp);
        SetValue(dest, value);
    }

    private long GetValue(string operand) {
        if (operand == "rsp") {
            return rsp;
        }
        
        if (long.TryParse(operand, out var immediateValue)) {
            return immediateValue;
        }

        if (operand.StartsWith("[") && operand.EndsWith("]")) {
            var address = long.Parse(operand.Trim('[', ']'));
            return memory[address];
        }

        switch (operand) {
            case "ra": return ra;
            case "rb": return rb;
            case "rc": return rc;
            case "rd": return rd;
            case "re": return re;
            case "rf": return rf;
            case "rg": return rg;
            case "rh": return rh;
            case "rsp": return rsp;
            default: throw new Exception($"Operand {operand} not recognized.");
        }
    }

    private void SetValue(string operand, long value) {
        if (operand.StartsWith("[") && operand.EndsWith("]")) {
            operand = operand.Trim('[', ']');
            if (operand == "rsp") {
                stackMemory[rsp] = value;
                return;
            }
            var address = long.Parse(operand);
            memory[address] = value;
            return;
        }

        switch (operand) {
            case "ra": ra = value; break;
            case "rb": rb = value; break;
            case "rc": rc = value; break;
            case "rd": rd = value; break;
            case "re": re = value; break;
            case "rf": rf = value; break;
            case "rg": rg = value; break;
            case "rh": rh = value; break;
            case "rsp": rsp = value; break;
            default: throw new Exception($"Operand {operand} not recognized.");
        }
    }
}