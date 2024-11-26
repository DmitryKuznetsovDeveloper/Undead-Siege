using Data.Character.WeaponsConfigs;
using Game.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Objects.Weapons
{
    public class MeleeWeapon : MonoBehaviour
    {
        [SerializeField] private Transform[] _meleePoints;
        [SerializeField, InlineEditor] private MeleeWeaponConfig _config;
        private readonly Collider[] _buffer = new Collider[32];
        
        [Button]
        public void MeleeAttack() => AttackProcess();

        private void AttackProcess()
        {
            Debug.Log("Weapon Attack");
            foreach (var meleePoint in _meleePoints)
            {
                Vector3 center = meleePoint.position; 
                int size = Physics.OverlapSphereNonAlloc(center, _config.meleeRadius, _buffer, _config.layerMask);

                if (_config.mode == MeleeWeaponConfig.Mode.First)
                {
                    for (int i = 0; i < size; i++)
                    {
                        Collider target = _buffer[i];
                        if (ProcessSingleHit(target)) break;
                    }
                }
                else if (_config.mode == MeleeWeaponConfig.Mode.All)
                    ProcessMultipleHits(size);
            }
        }

        private bool ProcessSingleHit(Collider target)
        {
            if (IsLayerMatch(target.gameObject.layer) && target.transform.TryGetComponent(out HealthComponent healthComponent))
            {
                healthComponent.TakeDamage(_config.damage);
                return true;
            }
            return false;
        }

        private void ProcessMultipleHits(int size)
        {
            for (int i = 0; i < size; i++)
            {
                Collider other = _buffer[i];
                if (IsLayerMatch(other.gameObject.layer) && other.transform.TryGetComponent(out HealthComponent healthComponent))
                    healthComponent.TakeDamage(_config.damage);
            }
        }

        private bool IsLayerMatch(int layer) => (_config.layerMask.value & (1 << layer)) != 0;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_meleePoints != null)
            {
                Gizmos.color = Color.red;
                foreach (var meleePoint in _meleePoints)
                    Gizmos.DrawWireSphere(meleePoint.position, _config.meleeRadius); // Рисуем Gizmos для всех точек
            }
        }
#endif
    }
}
