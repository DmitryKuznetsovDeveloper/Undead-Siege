using System;
using System.Collections.Generic;
using UnityEngine;

namespace Animations.Common
{
    [RequireComponent(typeof(Animator))]
    public sealed class AnimationEventListener : MonoBehaviour
    {
        public event Action<string> OnMessageReceived;
        private Dictionary<string, Action> _eventDictionary;
        
        private void Awake() => _eventDictionary = new Dictionary<string, Action>();
        
        public void AddEvent(string message, Action action)
        {
            Debug.Log(message);
            if (!_eventDictionary.TryAdd(message, action))
                _eventDictionary[message] += action;
        }
        
        public void RemoveEvent(string message, Action action)
        {
            if (_eventDictionary.ContainsKey(message))
            {
                _eventDictionary[message] -= action;
                
                if (_eventDictionary[message] == null)
                    _eventDictionary.Remove(message);
            }
        }
        
        public void ReceiveEvent(string message)
        {
            Debug.Log("ReceiveEvent");
            if (_eventDictionary.TryGetValue(message, out var action))
                action?.Invoke();
            
            OnMessageReceived?.Invoke(message);
        }
    }
}