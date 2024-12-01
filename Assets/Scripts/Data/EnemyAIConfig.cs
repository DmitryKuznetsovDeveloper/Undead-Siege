using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Game.Data
{
    [CreateAssetMenu(fileName = "EnemyAIConfig", menuName = "Configs/EnemyAIConfig", order = 0)]
    public class EnemyAIConfig : ScriptableObject
    {
        [Header("Movement Settings")]
        public float ArrivalDistance = 0.5f;
        public float PatrolRadius = 10f;
        public float WaitTimeBeforeNextPoint = 2;

        [Header("Attack Settings")]
        public float DetectionRange = 10f;
        public float AttackRange = 2f;
        public float AttackCooldown = 1f;

        [Header("Animations")]
        public ClipTransition IdleAnim;
        public ClipTransition WalkAnim;
        public ClipTransition RunAnim;
        public ClipTransition HitAnim;
        public ClipTransition ScreamAnim;
        public ClipTransition[] MeleeAttacksAnim;
        public float[] MeleeAttackEventTimes;
    }
}