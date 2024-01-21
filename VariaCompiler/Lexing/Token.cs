namespace VariaCompiler.Lexing;

public class Token {
    public TokenType Type { get; set; }
    public string Value { get; set; }

    
    public Token(TokenType type, string value) {
        this.Type = type;
        this.Value = value;
    }

    
    public override string ToString() {
        return $"{this.Type}: {this.Value}";
    }
}
