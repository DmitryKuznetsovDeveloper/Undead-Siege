using Animancer;
using Game.AI;
using Game.Components;
using Game.Data;
using Game.Objects.Weapons;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using VContainer.Unity;
namespace DI.Objects
{
    public class EnemyLifetimeScope : LifetimeScope
    {
        [SerializeField] private EnemyAIConfig _enemyAIConfig;
        
        protected override void Configure(IContainerBuilder builder)
        {
            //Components
            builder.RegisterInstance(_enemyAIConfig);
            builder.RegisterComponent(GetComponentInChildren<Animator>());
            builder.RegisterComponent(GetComponentInChildren<AnimancerComponent>());
            builder.RegisterComponent(GetComponentInChildren<AIAnimatorSync>());
            builder.RegisterComponent(GetComponentInChildren<NavMeshAgent>());
            builder.RegisterComponent(GetComponentInChildren<HealthComponent>());
            builder.RegisterComponent(GetComponent<MeleeWeapon>());

            //Controllers
            builder.Register<EnemyAIContext>(Lifetime.Singleton).AsSelf();
            builder.Register<PatrolState>(Lifetime.Singleton).As<IState<EnemyAIContext>>();
            builder.Register<ChaseState>(Lifetime.Singleton).As<IState<EnemyAIContext>>();
            builder.Register<AttackState>(Lifetime.Singleton).As<IState<EnemyAIContext>>();
            builder.Register<EnemyAIController>(Lifetime.Singleton).As<ITickable>().AsSelf();
            builder.Register<RagdollController>(Lifetime.Singleton).As<IInitializable>().AsSelf();
        }
    }
}