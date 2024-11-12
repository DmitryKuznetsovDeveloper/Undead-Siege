using Sirenix.OdinInspector;
using UnityEngine;
namespace Data.Character
{
    [CreateAssetMenu(fileName = "MouseControlConfig ", menuName = "Configs/Character/MouseControlConfig ", order = 0)]
    public class MouseControlConfig : ScriptableObject
    {
        [FoldoutGroup("Sensitivity Settings"), Tooltip("Чувствительность мыши по горизонтали")]
        public float MouseSensitivityX = 100f;

        [FoldoutGroup("Sensitivity Settings"), Tooltip("Чувствительность мыши по вертикали")]
        public float MouseSensitivityY = 100f;

        [FoldoutGroup("Sensitivity Settings"), Tooltip("Чувствительность геймпада по горизонтали")]
        public float GamepadSensitivityX = 600f;

        [FoldoutGroup("Sensitivity Settings"), Tooltip("Чувствительность геймпада по вертикали")]
        public float GamepadSensitivityY = 600f;

        [FoldoutGroup("Rotation Limits"), Tooltip("Максимальный угол поворота по вертикали вверх")]
        public float MaxVerticalAngle = 75f;

        [FoldoutGroup("Rotation Limits"), Tooltip("Максимальный угол поворота по вертикали вниз")]
        public float MinVerticalAngle = -75f;
    }
}