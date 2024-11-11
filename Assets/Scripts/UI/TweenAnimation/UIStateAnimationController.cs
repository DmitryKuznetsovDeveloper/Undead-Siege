using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
namespace UI.TweenAnimation
{
    public abstract class UIStateAnimationController<TState, TSettings, TConfig> : MonoBehaviour
        where TState : Enum
        where TConfig : ScriptableObject
        where TSettings : class
    {
        [SerializeField] protected TConfig _config;
        protected Dictionary<TState, Sequence> AnimationSequences;
        private TState _currentState;
        private bool _isFirstRun = true;

        public TState CurrentState => _currentState;

        private void OnDisable() => PauseAllSequences();

        private void OnDestroy() => KillAllSequences();

        protected virtual void PlayAnimation(TState state)
        {
            if (AnimationSequences == null)
            {
                _currentState = GetDefaultState();
                InitializeSequences();
            }

            if (_currentState.Equals(state) && !_isFirstRun) return;

            // Остановить текущую анимацию, если она есть
            if (AnimationSequences.TryGetValue(_currentState, out var currentSequence))
                currentSequence?.Pause();

            // Запустить новую анимацию
            if (AnimationSequences.TryGetValue(state, out var newSequence))
            {
                _currentState = state;
                _isFirstRun = false;
                newSequence?.Restart();
            }
        }

        protected async virtual UniTask PlayAnimationOnCompleted(TState state, Action action, CancellationToken cancellationToken)
        {
            if (AnimationSequences == null)
            {
                _currentState = GetDefaultState();
                InitializeSequences();
            }

            if (_currentState.Equals(state) && !_isFirstRun) return;

            // Остановить текущую анимацию, если она есть
            if (AnimationSequences.TryGetValue(_currentState, out var currentSequence))
                currentSequence?.Pause();

            // Запустить новую анимацию
            if (AnimationSequences.TryGetValue(state, out var newSequence))
            {
                _currentState = state;
                _isFirstRun = false;
                newSequence?.Restart();

                while (newSequence.IsActive() && !newSequence.IsComplete())
                {
                    // Проверка токена отмены
                    cancellationToken.ThrowIfCancellationRequested();
                    await UniTask.Yield();
                }
                action?.Invoke();
            }
        }


        protected abstract void InitializeSequences();

        protected abstract Sequence CreateSequence(TSettings stateConfig);

        protected abstract TState GetDefaultState();

        private void PauseAllSequences()
        {
            if (AnimationSequences == null) return;
            foreach (var sequences in AnimationSequences.Values)
                sequences?.Pause();
        }

        private void KillAllSequences()
        {
            if (AnimationSequences == null) return;
            foreach (var sequences in AnimationSequences.Values)
                sequences?.Kill();
        }
    }
}