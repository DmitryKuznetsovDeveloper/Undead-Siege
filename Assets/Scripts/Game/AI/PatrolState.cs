using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.AI
{
    public sealed class PatrolState : IState<EnemyAIContext>
    {
        public bool IsBusy => _isBusy;
        
        private bool _isBusy;
        private float _waitTimer;
        private Vector3 _nextPatrolPoint;
        private CancellationTokenSource _cancellationTokenSource;
        
        public void Enter(EnemyAIContext context)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _isBusy = false;
            _waitTimer = 0f;
            _nextPatrolPoint = GetNextPatrolPoint(context);

            if (_nextPatrolPoint != Vector3.zero) 
                context.Agent.SetDestination(_nextPatrolPoint);
        }

        public async void Execute(EnemyAIContext context)
        {
            if (_isBusy) return;

            try
            {
                if (HasReachedDestination(context))
                {
                    if (_isBusy) return;
                    
                    _isBusy = true;
                    _waitTimer = context.Config.WaitTimeBeforeNextPoint;
                    context.Animancer.Play(context.Config.IdleAnim);
                    await UniTask.Delay((int)(_waitTimer * 1000), cancellationToken: _cancellationTokenSource.Token);

                    _isBusy = false;

                    // Устанавливаем следующую точку патрулирования
                    _nextPatrolPoint = GetNextPatrolPoint(context);
                    context.Agent.SetDestination(_nextPatrolPoint);
                }
                else
                    context.Animancer.Play(context.Config.WalkAnim);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("PatrolState: Операция была отменена");
            }
        }

        public void Exit(EnemyAIContext context)
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            context.Agent.ResetPath();
            _isBusy = false;
        }

        private Vector3 GetNextPatrolPoint(EnemyAIContext context)
        {
            Vector3 randomDirection = Random.insideUnitSphere * context.Config.PatrolRadius;
            randomDirection += context.Agent.transform.position;

            if (NavMesh.SamplePosition(randomDirection, out var hit, context.Config.PatrolRadius, NavMesh.AllAreas))
                return hit.position;
            
            return context.Agent.transform.position; // Если точка не найдена, остаёмся на месте
        }

        private bool HasReachedDestination(EnemyAIContext context) => !context.Agent.pathPending && context.Agent.remainingDistance <= context.Config.ArrivalDistance;
    }
}