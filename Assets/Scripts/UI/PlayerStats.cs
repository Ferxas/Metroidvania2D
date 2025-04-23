using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 100;
    public float maxMana = 100;
    public float maxStamina = 100;

    public float currentHealth;
    public float currentMana;
    public float currentStamina;

    void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
    }

    public void UseMana(float amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
    }

    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
    }

    public void Regenerate()
    {
        currentMana = Mathf.Min(maxMana, currentMana + Time.deltaTime * 2f);
        currentStamina = Mathf.Min(maxStamina, currentStamina + Time.deltaTime * 4f);
    }
}
