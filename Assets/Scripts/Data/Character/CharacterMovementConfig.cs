using Sirenix.OdinInspector;
using UnityEngine;
namespace Data.Character
{
    [CreateAssetMenu(fileName = "CharacterMovementConfig", menuName = "Configs/Character/CharacterMovementConfig", order = 0)]
    public class CharacterMovementConfig : ScriptableObject
    {
        [FoldoutGroup("Movement Settings"), Tooltip("Скорость обычного передвижения персонажа")]
        public float MoveSpeed = 5f;

        [FoldoutGroup("Movement Settings"), Tooltip("Скорость бега")]
        public float RunSpeed = 10f;

        [FoldoutGroup("Jump Settings"), Tooltip("Сила прыжка")]
        public float JumpForce = 5f;

        [FoldoutGroup("Jump Settings"), Tooltip("Сила гравитации")]
        public float Gravity = -9.81f;

        [FoldoutGroup("Inertia Settings"), Tooltip("Скорость ускорения и замедления")]
        public float Acceleration = 15f;

        [FoldoutGroup("Rotation Settings"), Tooltip("Время сглаживания поворота")]
        public float RotationSmoothTime = 0.1f;
    }
}