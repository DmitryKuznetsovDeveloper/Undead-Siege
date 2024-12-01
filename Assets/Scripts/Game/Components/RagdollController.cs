using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Game.Components
{
    public sealed class RagdollController : IInitializable, IDisposable
    {
        private readonly HealthComponent _healthComponent;
        private readonly Animator _animator;
        private List<Rigidbody> _rigidbodies;
        public RagdollController(HealthComponent healthComponent, Animator animator)
        {
            _healthComponent = healthComponent;
            _animator = animator;
        }

        public void Initialize()
        {
            _rigidbodies = new List<Rigidbody>(_healthComponent.GetComponentsInChildren<Rigidbody>());
            Disabled();
            _healthComponent.OnResetDefault += Disabled;
            _healthComponent.OnDeath += Enable;
        }

        private void Enable()
        {
            _animator.enabled = false;
            foreach (var rb in _rigidbodies)
                rb.isKinematic = false;
        }
        private void Disabled()
        {
            _animator.enabled = true;
            foreach (var rb in _rigidbodies)
                rb.isKinematic = true;
        }
        public void Dispose()
        {
            _healthComponent.OnDeath -= Enable;
            _healthComponent.OnResetDefault -= Disabled;

        }
    }
}