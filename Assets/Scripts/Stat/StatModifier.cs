
[System.Serializable]
public struct StatModifier
{
    public float value;
    public object source;

    public StatModifier(float value, object source)
    {
        this.value = value;
        this.source = source;
    }
}