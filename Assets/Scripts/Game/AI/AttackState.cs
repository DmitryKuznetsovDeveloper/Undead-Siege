using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.AI
{
    public sealed class AttackState : IState<EnemyAIContext>
    {
        public bool IsBusy => _isBusy;
        
        private bool _isBusy;
        private CancellationTokenSource _cancellationTokenSource;

        public void Enter(EnemyAIContext context)
        {
            context.Agent.isStopped = true;
            _cancellationTokenSource = new CancellationTokenSource();
            _isBusy = false;
        }

        public async void Execute(EnemyAIContext context)
        {
            SmoothLookAt(context);

            if (_isBusy) return;

            _isBusy = true;

            try
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) return;

                // Случайный выбор анимации
                int randomIndex = Random.Range(0, context.Config.MeleeAttacksAnim.Length);
                var animation = context.Config.MeleeAttacksAnim[randomIndex];
                float attackTime = context.Config.MeleeAttackEventTimes[randomIndex];
                var animState = context.Animancer.Play(animation);
                animState.Events(this).Add(attackTime, () =>
                {
                    Debug.Log("AttackState: Нанесение урона");
                    context.MeleeWeapon.MeleeAttack();
                });
                await animState.ToUniTask(cancellationToken: _cancellationTokenSource.Token);
                await UniTask.Delay((int)(context.Config.AttackCooldown * 1000), cancellationToken: _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("AttackState: Выполнение отменено");
            }
            finally
            {
                _isBusy = false;
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

            _isBusy = false;
            context.Agent.isStopped = false;
        }

        private void SmoothLookAt(EnemyAIContext context)
        {
            var enemyTransform = context.Animancer.transform;
            var playerPosition = context.Player.transform.position;
            const float rotationSpeed = 2f;
            var targetDirection = playerPosition - enemyTransform.position;
            targetDirection.y = 0; // Убираем вертикальный наклон
            var targetRotation = Quaternion.LookRotation(targetDirection);

            enemyTransform.rotation = Quaternion.Lerp(
                enemyTransform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed);
        }
    }
}
