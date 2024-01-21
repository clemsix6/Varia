using System.Globalization;


namespace VariaCompiler.Interpreter;


public enum RegisterType
{
    Int,
    Float,
    Null
}



public class Register
{
    private long? IntValue { get; set; } = null;
    private double? FloatValue { get; set; } = null;


    public RegisterType Type
    {
        get {
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
    }


    public Register(long intValue)
    {
        this.IntValue = intValue;
        this.FloatValue = null;
    }


    public Register(double floatValue)
    {
        this.IntValue = null;
        this.FloatValue = floatValue;
    }


    public Register Add(long value)
    {
        if (this.Type == RegisterType.Int)
            this.IntValue += value;
        else if (this.Type == RegisterType.Float)
            this.FloatValue += value;
        else
            this.IntValue = value;
        return this;
    }


    public Register Add(double value)
    {
        if (this.Type == RegisterType.Int)
            this.IntValue = null;
        if (this.FloatValue.HasValue)
            this.FloatValue += value;
        else
            this.FloatValue = value;
        return this;
    }


    public Register Add(Register other)
    {
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Int) {
            this.IntValue += other.IntValue;
        } else if (this.Type == RegisterType.Float && other.Type == RegisterType.Float) {
            this.FloatValue += other.FloatValue;
        } else if (this.Type == RegisterType.Int && other.Type == RegisterType.Float) {
            this.FloatValue = this.IntValue + other.FloatValue;
            this.IntValue = null;
        } else if (this.Type == RegisterType.Float && other.Type == RegisterType.Int) {
            this.FloatValue += other.IntValue;
        } else if (this.Type == RegisterType.Null) {
            Set(other);
        } else {
            throw new InvalidOperationException("Cannot add registers of different types.");
        }
        return this;
    }


    public Register Sub(long value)
    {
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
        if (this.Type == RegisterType.Int && other.Type == RegisterType.Int) {
            this.IntValue -= other.IntValue;
        } else if (this.Type == RegisterType.Float && other.Type == RegisterType.Float) {
            this.FloatValue -= other.FloatValue;
        } else if (this.Type == RegisterType.Int && other.Type == RegisterType.Float) {
            this.FloatValue = this.IntValue - other.FloatValue;
            this.IntValue = null;
        } else if (this.Type == RegisterType.Float && other.Type == RegisterType.Int) {
            this.FloatValue -= other.IntValue;
        } else if (this.Type == RegisterType.Null) {
            Set(other);
        } else {
            throw new InvalidOperationException("Cannot subtract registers of different types.");
        }
        return this;
    }


    public void Set(Register other)
    {
        this.IntValue = other.IntValue;
        this.FloatValue = other.FloatValue;
    }


    public void Set(long intValue)
    {
        this.IntValue = intValue;
        this.FloatValue = null;
    }


    public void Set(double floatValue)
    {
        this.IntValue = null;
        this.FloatValue = floatValue;
    }


    public string? GetStringValue()
    {
        if (this.IntValue.HasValue)
            return this.IntValue.Value.ToString();
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
        throw new InvalidOperationException("Cannot get int value of register with no value.");
    }


    public double GetFloatValue()
    {
        if (this.IntValue.HasValue)
            return this.IntValue.Value;
        if (this.FloatValue.HasValue)
            return this.FloatValue.Value;
        throw new InvalidOperationException("Cannot get float value of register with no value.");
    }


    public Register Clone()
    {
        var clone = new Register();
        clone.Set(this);
        return clone;
    }
}