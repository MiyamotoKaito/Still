using Still.Enum.CompareType;

public struct Condition
{
    public int Value;
    public CompareType Comparison;

    public Condition(int value, CompareType comparison)
    {
        Value = value;
        Comparison = comparison;
    }
}