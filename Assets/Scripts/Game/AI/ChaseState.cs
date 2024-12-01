using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.AI
{
    public sealed class ChaseState : IState<EnemyAIContext>
    {
        public bool IsBusy => _isBusy;

        private bool _isBusy;
        private CancellationTokenSource _cancellationTokenSource;

        public async void Enter(EnemyAIContext context)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            context.Agent.isStopped = false;
            try
            {
                _isBusy = true;
                // Анимация с использованием токена отмены
                await context.Animancer.Play(context.Config.ScreamAnim).ToUniTask(cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("ChaseState: Анимация была отменена");
            }
            finally
            {
                _isBusy = false;
            }
        }

        public void Execute(EnemyAIContext context)
        {
            if (_isBusy) return;
            
            if (context.Agent.destination != context.Player.transform.position)
                context.Agent.SetDestination(context.Player.transform.position);

            context.Animancer.Play(context.Config.RunAnim);
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
        }
    }
}