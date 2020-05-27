using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthPool : MonoBehaviour {
    
    [SerializeField] private int maxHealth;
    [SerializeField] private float invincibilityDuration;

    private float _invincible;
    private int _currentHealth;
    public int CurrentHealth => _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void Damage()
    {
        if (_invincible > 0) return;
        
        _currentHealth--;
        if (_currentHealth == 0)
        {
            Death();
        } else
        {
            Invincibility(-1);
        }
    }

    public void Heal()
    {
        if (_currentHealth < maxHealth) _currentHealth++;
    }

    public void Cleanse()
    {
        _currentHealth = maxHealth;
    }
    
    public void Death()
    {
        Debug.Log("Handle death");
    }

    public void Invincibility(float time)
    {
        if (time < 0) time = invincibilityDuration;
        _invincible = time;
    }

    private void Update()
    {
        if (_invincible > 0)
        {
            _invincible -= Time.deltaTime;
        }
    }
}
