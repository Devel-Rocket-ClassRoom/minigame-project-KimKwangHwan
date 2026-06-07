using System;
using System.Collections.Generic;
using System.Linq;

public class StatContainer
{
    private float baseValue;
    private List<StatModifier> modifiers = new List<StatModifier>();
    private float cachedValue;
    private bool isDirty = false;

    public event Action<float> OnValueChanged;

    public StatContainer(float value)
    {
        this.baseValue = value;
        this.cachedValue = value;
        isDirty = false;
    }

    public float FinalValue
    {
        get
        {
            if (isDirty)
            {
                cachedValue = ReCalculate();
                isDirty = false;
            }
            return cachedValue;
        }
    }

    public void SetBaseValue(float value)
    {
        baseValue = value;
        isDirty = true;
        OnValueChanged?.Invoke(FinalValue);
    }

    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
        isDirty = true;
        OnValueChanged?.Invoke(FinalValue);
    }

    public void RemoveBySource(object source)
    {
        modifiers.RemoveAll(m => m.source == source);
        isDirty = true;
        OnValueChanged?.Invoke(FinalValue);
    }

    private float ReCalculate()
    {
        float finalValue = baseValue;

        foreach (var stat in modifiers.ToList())
        {
            finalValue += stat.value;
        }

        return finalValue;
    }
}