using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace Game.AI
{
    public class EnemyAIController : ITickable
    {
        private readonly EnemyAIContext _context;
        private readonly List<IState<EnemyAIContext>> _states;
        private IState<EnemyAIContext> _currentState;
        private float _distanceToPlayer;
        private float _stateDurationTimer;

        public EnemyAIController(EnemyAIContext context, IEnumerable<IState<EnemyAIContext>> states)
        {
            _context = context;
            _states = new List<IState<EnemyAIContext>>(states);

            // Устанавливаем начальное состояние (PatrolState)
            _currentState = _states.Find(s => s is PatrolState) ?? throw new System.Exception("PatrolState не найден!");
            _currentState.Enter(_context);
        }

        public  void Tick()
        {
            if (_context.HealthComponent.CurrentHealthPoints <= 0 || !_context.Agent.gameObject.activeInHierarchy ) return;
            
            // Обновляем дистанцию до игрока
            _distanceToPlayer = Vector3.Distance(_context.Agent.transform.position, _context.Player.transform.position);

            // Обновляем таймер текущего состояния
            _stateDurationTimer -= Time.deltaTime;

            // Проверяем переходы только если таймер истёк
            if (_stateDurationTimer <= 0f)
            {
                HandleTransitions();
                _stateDurationTimer = 0.5f; // Минимальная длительность состояния
            }
            
            // Выполняем логику текущего состояния
             _currentState?.Execute(_context);
        }

        private void HandleTransitions()
        {
            switch (_currentState)
            {
                case PatrolState when _distanceToPlayer <= _context.Config.DetectionRange:
                    TransitionTo<ChaseState>();
                    break;

                case ChaseState:
                    if (_distanceToPlayer <= _context.Config.AttackRange)
                        TransitionTo<AttackState>();
                    
                    else if (_distanceToPlayer > _context.Config.DetectionRange)
                        TransitionTo<PatrolState>();
                    break;

                case AttackState:
                    if (_distanceToPlayer > _context.Config.AttackRange && _distanceToPlayer <= _context.Config.DetectionRange)
                        TransitionTo<ChaseState>();
                    
                    else if (_distanceToPlayer > _context.Config.DetectionRange)
                        TransitionTo<PatrolState>();
                    break;
            }
        }

        private void TransitionTo<TState>() where TState : IState<EnemyAIContext>
        {
            var newState = _states.Find(s => s is TState);
            if (newState == null)
            {
                Debug.LogError($"Не удалось найти состояние {typeof(TState).Name}");
                return;
            }

            Debug.Log($"Переход из {_currentState.GetType().Name} в {newState.GetType().Name}");
            _currentState.Exit(_context);
            _currentState = newState;
            _currentState.Enter(_context);
        }
    }
}
