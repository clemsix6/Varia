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
        if (this.Type == RegisterType.String || other.Type == RegisterType.String) {
            Set(GetStringValue() + other.GetStringValue());
            return this;
        }
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Int) {
            Set(GetIntValue() + other.GetIntValue());
            return this;
        }
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Float) {
            Set(GetIntValue() + other.GetFloatValue());
            return this;
        }
        if (this.Type == RegisterType.Float) {
            Set(this.FloatValue!.Value + other.GetFloatValue());
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
        if (this.Type == RegisterType.String || other.Type == RegisterType.String)
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


    public Register Mul(long value)
    {
        if (this.Type == RegisterType.String)
            throw new InvalidOperationException("Cannot multiply string.");
        if (this.Type == RegisterType.Int)
            this.IntValue *= value;
        else if (this.Type == RegisterType.Float)
            this.FloatValue *= value;
        else
            this.IntValue = 0;
        return this;
    }


    public Register Mul(double value)
    {
        if (this.Type == RegisterType.String)
            throw new InvalidOperationException("Cannot multiply string.");
        if (this.Type == RegisterType.Int)
            this.FloatValue = this.IntValue * value;
        else if (this.Type == RegisterType.Float)
            this.FloatValue *= value;
        else
            this.IntValue = 0;
        return this;
    }


    public Register Mul(Register other)
    {
        if (this.Type == RegisterType.String || other.Type == RegisterType.String)
            throw new InvalidOperationException("Cannot multiply string.");
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Int) {
            this.IntValue *= other.GetIntValue();
            return this;
        }
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Float) {
            this.FloatValue = this.IntValue * other.GetFloatValue();
            this.IntValue = null;
            return this;
        }
        if (this.Type == RegisterType.Float && other.Type == RegisterType.Float) {
            this.FloatValue *= other.GetFloatValue();
            return this;
        }
        if (this.Type == RegisterType.Float && other.Type == RegisterType.Int) {
            this.FloatValue *= other.GetIntValue();
            return this;
        }
        throw new InvalidOperationException("Cannot multiply register of type " + this.Type);
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


    public string? GetStringValue()
    {
        if (this.StringValue != null)
            return this.StringValue;
        if (this.IntValue != null)
            return this.IntValue.Value.ToString(CultureInfo.InvariantCulture);
        if (this.FloatValue != null)
            return this.FloatValue.Value.ToString(CultureInfo.InvariantCulture);
        return null;
    }


    public Register Clone()
    {
        var clone = new Register();
        clone.Set(this);
        return clone;
    }


    public override bool Equals(object? obj)
    {
        if (obj is Register other) {
            if (this.Type != other.Type)
                return false;
            return this.Type switch
            {
                RegisterType.Int => GetIntValue() == other.GetIntValue(),
                RegisterType.Float => GetFloatValue() == other.GetFloatValue(),
                RegisterType.String => GetStringValue() == other.GetStringValue(),
                RegisterType.Null => true,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        return false;
    }


    protected bool Equals(Register other)
    {
        return this.IntValue == other.IntValue &&
               Nullable.Equals(this.FloatValue, other.FloatValue) &&
               this.StringValue == other.StringValue;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(GetIntValue(), GetFloatValue(), GetStringValue());
    }


    public bool LessThan(Register other)
    {
        return this.Type switch
        {
            RegisterType.Int => this.GetIntValue() < other.GetIntValue(),
            RegisterType.Float => this.GetFloatValue() < other.GetFloatValue(),
            RegisterType.String => string.Compare(
                                       this.GetStringValue(), other.GetStringValue(),
                                       StringComparison.Ordinal
                                   ) <
                                   0,
            _ => throw new InvalidOperationException("Cannot compare Null type.")
        };
    }


    public bool GreaterThan(Register other)
    {
        return this.Type switch
        {
            RegisterType.Int => this.GetIntValue() > other.GetIntValue(),
            RegisterType.Float => this.GetFloatValue() > other.GetFloatValue(),
            RegisterType.String => string.Compare(
                                       this.GetStringValue(), other.GetStringValue(),
                                       StringComparison.Ordinal
                                   ) >
                                   0,
            _ => throw new InvalidOperationException("Cannot compare Null type.")
        };
    }


    public bool LessThanOrEqual(Register other)
    {
        return LessThan(other) || Equals(other);
    }


    public bool GreaterThanOrEqual(Register other)
    {
        return GreaterThan(other) || Equals(other);
    }
}