using Still.Enum.CompareType;
[System.Serializable]
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