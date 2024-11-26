using Sirenix.OdinInspector;
using UnityEngine;
namespace Data.Character.WeaponsConfigs
{
    [CreateAssetMenu(fileName = "MeleeWeaponConfig", menuName = "Configs/Weapon/MeleeWeaponConfig")]
    public sealed class MeleeWeaponConfig : ScriptableObject
    {
        [BoxGroup("Damage Settings")]
        [LabelWidth(100)]
        [Tooltip("Урон от оружия")]
        public int damage = 1;

        [BoxGroup("Mode Settings")]
        [LabelWidth(100)]
        [Tooltip("Режим атаки: First — по первому попавшемуся, All — по всем целям")]
        public Mode mode = Mode.First;

        [BoxGroup("Radius Settings")]
        [LabelWidth(100)]
        [Tooltip("Радиус атаки ближнего боя")]
        public float meleeRadius = 0.2f;

        [BoxGroup("Layer Settings")]
        [LabelWidth(100)]
        [Tooltip("Слои, по которым оружие будет наносить урон")]
        public LayerMask layerMask;

        public enum Mode { First = 0, All = 1 }
    }
}