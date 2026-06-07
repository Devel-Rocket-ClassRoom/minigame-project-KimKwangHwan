using System;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float genAmount = 0.2f;
    [SerializeField] private float genStartInterval;
    [SerializeField] private float genInterval;

    private PlayerStats _stats;
    private PlayerStats Stats => _stats != null ? _stats : (_stats = GetComponent<PlayerStats>());

    private float currentStamina;
    public float MaxStamina => Stats.MaxStamina.FinalValue;
    public float CurrentStamina => currentStamina;
    public event Action<float, float> OnStaminaChanged;

    private bool useStamina = false;
    private float genStartTimer;
    private float genTimer;

    private void Awake()
    {
        currentStamina = MaxStamina;
        genStartTimer = 0f;
        genTimer = 0f;
        Stats.MaxStamina.OnValueChanged += OnMaxStaminaChanged;
    }

    private void OnDestroy()
    {
        if (_stats != null)
            _stats.MaxStamina.OnValueChanged -= OnMaxStaminaChanged;
    }

    private void OnMaxStaminaChanged(float newMax)
    {
        currentStamina = Mathf.Min(currentStamina, newMax);
        OnStaminaChanged?.Invoke(currentStamina, MaxStamina);
    }

    public void ForceNotify() => OnStaminaChanged?.Invoke(currentStamina, MaxStamina);

    public bool TryUseStamina(float amount)
    {
        if (currentStamina < amount)
            return false;
        currentStamina -= amount;
        if (currentStamina < 0f)
            currentStamina = 0f;
        OnStaminaChanged?.Invoke(currentStamina, MaxStamina);
        useStamina = true;
        genStartTimer = 0f;
        return true;
    }

    private void Update()
    {
        if (useStamina)
        {
            genStartTimer += Time.deltaTime;
            if (genStartTimer >= genStartInterval)
            {
                useStamina = false;
                genStartTimer = 0f;
            }
            return;
        }
        if (currentStamina < MaxStamina)
        {
            genTimer += Time.deltaTime;
            if (genTimer >= genInterval)
            {
                genTimer = 0f;
                currentStamina += genAmount;
                if (currentStamina > MaxStamina)
                    currentStamina = MaxStamina;
                OnStaminaChanged?.Invoke(currentStamina, MaxStamina);
            }
        }
    }
}
