using Animancer;
using UnityEngine;
namespace Data.Character.WeaponsConfigs
{
    [CreateAssetMenu(fileName = "MeleeWeaponAnimationConfig", menuName = "Configs/Weapon/MeleeWeaponAnimationConfig", order = 0)]
    public class MeleeWeaponAnimationConfig : ScriptableObject
    {
        public ClipTransition IDLETransition;
        public ClipTransition ShowTransition;
        public ClipTransition HideTransition;
        public ClipTransition[] MeleeAttackTransitions;
    }
}