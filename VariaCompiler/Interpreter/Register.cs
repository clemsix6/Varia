using System.Globalization;


namespace VariaCompiler.Interpreter;


public enum RegisterType
{
    Null,
    Int,
    Float,
    String,
}



public class Register
{
    private long? IntValue { get; set; } = null;
    private double? FloatValue { get; set; } = null;
    private string? StringValue { get; set; } = null;

    private RegisterType Type
    {
        get {
            if (!string.IsNullOrEmpty(this.StringValue))
                return RegisterType.String;
            if (this.IntValue.HasValue)
                return RegisterType.Int;
            if (this.FloatValue.HasValue)
                return RegisterType.Float;
            return RegisterType.Null;
        }
    }


    public Register()
    {
        this.IntValue = null;
        this.FloatValue = null;
        this.StringValue = null;
    }


    public Register(long intValue)
    {
        this.IntValue = intValue;
        this.FloatValue = null;
        this.StringValue = null;
    }


    public Register(double floatValue)
    {
        this.IntValue = null;
        this.FloatValue = floatValue;
        this.StringValue = null;
    }


    public Register(string stringValue)
    {
        this.IntValue = null;
        this.FloatValue = null;
        this.StringValue = stringValue;
    }


    public Register Add(long value)
    {
        switch (this.Type) {
            case RegisterType.Int:
                Set(this.IntValue!.Value + value);
                return this;
            case RegisterType.Float:
                Set(this.FloatValue!.Value + value);
                return this;
            case RegisterType.String:
                Set(this.StringValue + value);
                return this;
            case RegisterType.Null:
                Set(value);
                return this;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public Register Add(double value)
    {
        switch (this.Type) {
            case RegisterType.Int:
                Set(this.IntValue!.Value + value);
                return this;
            case RegisterType.Float:
                Set(this.FloatValue!.Value + value);
                return this;
            case RegisterType.String:
                Set(this.StringValue + value);
                return this;
            case RegisterType.Null:
                Set(value);
                return this;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public Register Add(string value)
    {
        switch (this.Type) {
            case RegisterType.Int:
                Set(this.IntValue!.Value + value);
                return this;
            case RegisterType.Float:
                Set(this.FloatValue!.Value + value);
                return this;
            case RegisterType.String:
                Set(this.StringValue + value);
                return this;
            case RegisterType.Null:
                Set(value);
                return this;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public Register Add(Register other)
    {
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Int) {
            Set(this.IntValue!.Value + other.GetIntValue());
            return this;
        }
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Float) {
            Set(this.IntValue!.Value + other.GetFloatValue());
            return this;
        }
        if (this.Type == RegisterType.Int && other.Type == RegisterType.String) {
            Set(this.IntValue!.Value + other.GetStringValue());
            return this;
        }
        if (this.Type == RegisterType.Float && other.Type == RegisterType.String) {
            Set(this.FloatValue!.Value + other.GetStringValue());
            return this;
        }
        if (this.Type == RegisterType.Float) {
            Set(this.FloatValue!.Value + other.GetFloatValue());
            return this;
        }
        if (this.Type == RegisterType.String) {
            Set(this.StringValue + other.GetStringValue());
            return this;
        }
        throw new InvalidOperationException("Cannot add register of type " + this.Type);
    }


    public Register Sub(long value)
    {
        if (this.Type == RegisterType.String)
            throw new InvalidOperationException("Cannot subtract from string.");
        if (this.Type == RegisterType.Int)
            this.IntValue -= value;
        else if (this.Type == RegisterType.Float)
            this.FloatValue -= value;
        else
            this.IntValue = -value;
        return this;
    }


    public Register Sub(double value)
    {
        if (this.Type == RegisterType.String)
            throw new InvalidOperationException("Cannot subtract from string.");
        if (this.Type == RegisterType.Int)
            this.IntValue = null;
        if (this.FloatValue.HasValue)
            this.FloatValue -= value;
        else
            this.FloatValue = -value;
        return this;
    }


    public Register Sub(Register other)
    {
        if (this.Type == RegisterType.String)
            throw new InvalidOperationException("Cannot subtract from string.");
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Int) {
            this.IntValue -= other.GetIntValue();
            return this;
        }
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Float) {
            this.FloatValue = this.IntValue - other.GetFloatValue();
            this.IntValue = null;
            return this;
        }
        if (this.Type == RegisterType.Float && other.Type == RegisterType.Float) {
            this.FloatValue -= other.GetFloatValue();
            return this;
        }
        if (this.Type == RegisterType.Float && other.Type == RegisterType.Int) {
            this.FloatValue -= other.GetIntValue();
            return this;
        }
        throw new InvalidOperationException("Cannot subtract register of type " + this.Type);
    }


    public void Set(Register other)
    {
        this.IntValue = other.IntValue;
        this.FloatValue = other.FloatValue;
        this.StringValue = other.StringValue;
    }


    public void Set(long intValue)
    {
        this.IntValue = intValue;
        this.FloatValue = null;
        this.StringValue = null;
    }


    public void Set(double floatValue)
    {
        this.IntValue = null;
        this.FloatValue = floatValue;
        this.StringValue = null;
    }


    public void Set(string stringValue)
    {
        this.IntValue = null;
        this.FloatValue = null;
        this.StringValue = stringValue;
    }


    public string? GetStringValue()
    {
        if (this.StringValue != null)
            return this.StringValue;
        if (this.IntValue.HasValue)
            return this.IntValue.Value.ToString(CultureInfo.InvariantCulture);
        if (this.FloatValue.HasValue)
            return this.FloatValue.Value.ToString(CultureInfo.InvariantCulture);
        return null;
    }


    public long GetIntValue()
    {
        if (this.IntValue.HasValue)
            return this.IntValue.Value;
        if (this.FloatValue.HasValue)
            return (long)this.FloatValue.Value;
        if (this.StringValue != null && long.TryParse(this.StringValue, out var result))
            return result;
        throw new InvalidOperationException(
            "Cannot get int value of register of type " + this.Type
        );
    }


    public double GetFloatValue()
    {
        if (this.IntValue.HasValue)
            return this.IntValue.Value;
        if (this.FloatValue.HasValue)
            return this.FloatValue.Value;
        if (this.StringValue != null && double.TryParse(this.StringValue, out var result))
            return result;
        throw new InvalidOperationException(
            "Cannot get int value of register of type " + this.Type
        );
    }


    public Register Clone()
    {
        var clone = new Register();
        clone.Set(this);
        return clone;
    }
}