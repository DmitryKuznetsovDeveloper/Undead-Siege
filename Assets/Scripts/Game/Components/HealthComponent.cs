using System;
using UnityEngine;

namespace Game.Components
{
    public sealed class HealthComponent : MonoBehaviour
    {
        public event Action<int> OnChangeHealth;
        public event Action<int> OnTakeDamage;
        public event Action OnDeath;
        public int MaxHitPoints => _maxHitPoints;
        public int CurrentHealthPoints => _health;
        
        [SerializeField, Range(0,100)] private int _maxHitPoints = 100;
        [SerializeField, Range(0,100)] private int _health = 50;
        
        private void Awake() =>
            OnChangeHealth?.Invoke(_health);
        
        public void TakeDamage(int damage)
        {
            _health = Math.Max(0, _health - damage);
            OnChangeHealth?.Invoke(_health);
            
            if (_health <= 0) 
                OnDeath?.Invoke();
            else 
                OnTakeDamage?.Invoke(damage);
        }
        
        public void RestoreHitPoints(int healthPoints)
        {
            _health = healthPoints > 0 ? Mathf.Min(_health + healthPoints, _maxHitPoints) : _health;
            OnChangeHealth?.Invoke(healthPoints);   
        }
    }
}