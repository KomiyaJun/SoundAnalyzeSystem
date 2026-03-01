using System;
using UnityEngine;

public class Health : MonoBehaviour, Idamageable
{
    [SerializeField] private int maxHealth = 3;
    private int _currentHealth;

    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    public int CurrentHealth => _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    public void TakeDamage(int amount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);

        OnHealthChanged?.Invoke(_currentHealth);

        if(_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }

    }

    public void ResetHealth()
    {
        _currentHealth = maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
