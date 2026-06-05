using System;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 50;
    [SerializeField] private float genAmount = 0.2f;
    private float currentStamina;
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public event Action<float, float> OnStaminaChanged;
    private bool useStamina = false;
    private float genStartTimer;
    private float genTimer;
    [SerializeField] private float genStartInterval;
    [SerializeField] private float genInterval;
    private void Awake()
    {
        currentStamina = maxStamina;
        genStartTimer = 0f;
        genTimer = 0f;
    }

    public void ForceNotify() => OnStaminaChanged?.Invoke(currentStamina, maxStamina);

    public bool TryUseStamina(float amount)
    {
        if (currentStamina < amount)
            return false;
        currentStamina -= amount;
        if (currentStamina < 0f)
            currentStamina = 0f;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
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
        if (currentStamina < maxStamina)
        {
            genTimer += Time.deltaTime;
            if (genTimer >= genInterval)
            {
                genTimer = 0f;
                currentStamina += genAmount;
                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
        }
    }
}
