using System.Text.RegularExpressions;


namespace VariaCompiler.Interpreter
{
    public class VirtualMachine
    {
        private Register ra, rb, rc, rd, re, rf, rg, rh, rsp;
        private readonly Dictionary<long, Register> memory = new();
        private readonly Dictionary<long, Register> stackMemory = new();
        private readonly Dictionary<string, List<string>> functions = new();
        private int currentInstructionIndex;

        private readonly Regex movPattern = new(
            @"^\s*mov\s+(""[^""]*""|\[\w+\]|\w+)\s+(\[\w+\]|\w+)\s*$", RegexOptions.IgnoreCase
        );
        private readonly Regex addPattern = new(
            @"^\s*add\s+([\w\d\[\].]+?)\s+([\w\d\[\].]+?)\s*$", RegexOptions.IgnoreCase
        );
        private readonly Regex pushPattern = new(
            @"^\s*push\s+([\w\d\[\].]+?)\s*$", RegexOptions.IgnoreCase
        );
        private readonly Regex popPattern = new(
            @"^\s*pop\s+([\w\d\[\]]+?)\s*$", RegexOptions.IgnoreCase
        );
        private readonly Regex retPattern = new(@"^\s*ret\s*$", RegexOptions.IgnoreCase);
        private readonly Regex callPattern = new(
            @"^\s*call\s+([\w\d\[\].]+?)\s*$", RegexOptions.IgnoreCase
        );


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


        public string? Execute(List<string> blueCodeInstructions)
        {
            ParseFunctions(blueCodeInstructions);
            ExecuteFunction("main");
            return ra.GetStringValue();
        }


        private void ParseFunctions(List<string> instructions)
        {
            var currentFunction = "";
            foreach (var instruction in instructions) {
                if (instruction.StartsWith("--")) {
                    currentFunction = instruction[2..];
                    this.functions[currentFunction] = new List<string>();
                } else {
                    this.functions[currentFunction].Add(instruction);
                }
            }
        }


        private bool ExecuteBuiltIn(string functionName)
        {
            switch (functionName) {
                case "print":
                    Console.WriteLine(this.ra.GetStringValue());
                    return true;
                case "exit":
                    Environment.Exit((int)this.ra.GetIntValue());
                    return true;
                default:
                    return false;
            }
        }


        private void ExecuteFunction(string functionName)
        {
            var instructions = this.functions[functionName];
            for (this.currentInstructionIndex = 0;
                 this.currentInstructionIndex < instructions.Count;
                 this.currentInstructionIndex++) {
                var instruction = instructions[this.currentInstructionIndex];

                var match = this.movPattern.Match(instruction);
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

                match = this.retPattern.Match(instruction);
                if (match.Success) {
                    if (functionName != "main") {
                        this.currentInstructionIndex =
                            (int)this.stackMemory[this.rsp.GetIntValue() - 1].GetIntValue();
                        this.rsp.Sub(1);
                    }
                    break;
                }

                match = this.callPattern.Match(instruction);
                if (match.Success) {
                    HandleCall(match.Groups[1].Value);
                    continue;
                }

                throw new Exception($"Instruction '{instruction}' not recognized.");
            }
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


        private void HandleCall(string functionName)
        {
            this.stackMemory[this.rsp.GetIntValue()] =
                new Register(this.currentInstructionIndex + 1);
            this.rsp.Add(1);
            if (!ExecuteBuiltIn(functionName))
                ExecuteFunction(functionName);
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
            if (operand.StartsWith("\"") && operand.EndsWith("\"")) {
                return new Register(operand.Trim('\"'));
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
                _ => throw new Exception($"Operand '{operand}' not recognized."),
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
                    this.ra.Set(value);
                    break;
                case "rb":
                    this.rb.Set(value);
                    break;
                case "rc":
                    this.rc.Set(value);
                    break;
                case "rd":
                    this.rd.Set(value);
                    break;
                case "re":
                    this.re.Set(value);
                    break;
                case "rf":
                    this.rf.Set(value);
                    break;
                case "rg":
                    this.rg.Set(value);
                    break;
                case "rh":
                    this.rh.Set(value);
                    break;
                case "rsp":
                    this.rsp.Set(value);
                    break;
                default:
                    throw new Exception($"Operand '{operand}' not recognized.");
            }
        }
    }
}