using Animancer;
using Game.Components;
using Game.Data;
using Game.Objects.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI
{
    public sealed class EnemyAIContext
    {
        public NavMeshAgent Agent { get; }
        public AnimancerComponent Animancer { get; }
        public EnemyAIConfig Config { get; }
        public MeleeWeapon MeleeWeapon { get; } 
        public CharacterController Player { get; }
        public HealthComponent HealthComponent { get; }
        
        public EnemyAIContext(NavMeshAgent agent, AnimancerComponent animancer, EnemyAIConfig config, CharacterController player, MeleeWeapon meleeWeapon, HealthComponent healthComponent)
        {
            Agent = agent;
            Animancer = animancer;
            Config = config;
            Player = player;
            MeleeWeapon = meleeWeapon;
            HealthComponent = healthComponent;
        }
    }
}