using System;
using System.Threading;
using Animancer;
using Animations.Common;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Data.Character.WeaponsConfigs;
using Game.Components;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Game.Objects.Weapons
{
    public abstract class ShootingWeapon : MeleeWeapon
    {
        public event Action<int,string,int> OnChangeAmmo;
        public bool IsReload => _isReload;

        public ShootingWeaponConfig WeaponConfig => _weaponConfig;
        public CinemachineVirtualCamera BaseCamera => _baseCamera;
        
        [SerializeField] private Transform _barrelTransform;
        [SerializeField,InlineEditor] protected ShootingWeaponConfig _weaponConfig;
        [SerializeField] protected CinemachineVirtualCamera _baseCamera;
        [SerializeField] protected LayerMask _characterLayerMask;
        [SerializeField] protected AnimancerComponent _animancer;
        [SerializeField] private ClipTransition IdleAnim;
        [SerializeField] private ClipTransition ShowAnim;
        [SerializeField] private ClipTransition HideAnim;
        [SerializeField] private ClipTransition MeleeAttackAnim; 
        [SerializeField] private ClipTransition ShootAnim;
        [SerializeField] private ClipTransition EmptyClipAnim;
        [SerializeField] private ClipTransition ReloadAnim;

        protected int currentClip;
        protected int totalAmmo;
        
        private float _nextTimeToFire;
        private bool _isReload;
        private CancellationTokenSource _cancellationTokenSource;

        protected virtual void Awake() => InitializeWeapon();

        protected void OnEnable() => _cancellationTokenSource = new CancellationTokenSource();
        
        protected void OnDisable() => _cancellationTokenSource.Cancel();
        
        protected void OnDestroy() => _cancellationTokenSource.Cancel();
        
        protected virtual void InitializeWeapon()
        {
            currentClip = _weaponConfig.MagazineCapacity;
            totalAmmo = _weaponConfig.TotalAmmo;
        }
        
        public async UniTask ShowWeapon() =>  await _animancer.Play(ShowAnim).ToUniTask(cancellationToken: _cancellationTokenSource.Token);
        
        public async UniTask HideWeapon() => await _animancer.Play(HideAnim).ToUniTask(cancellationToken: _cancellationTokenSource.Token);

        public void MeleeAttackAnimation()
        {
            var shootState = _animancer.Play(MeleeAttackAnim);
            shootState.Events(this).OnEnd = () => _animancer.Play(IdleAnim);
        }

        public void Shoot()
        {
            if (Time.time >= _nextTimeToFire && !_isReload && currentClip > 0)
            {
                var shootState = _animancer.Play(ShootAnim);
                shootState.Events(this).OnEnd = () => _animancer.Play(IdleAnim);
                ShootProcess();
                _nextTimeToFire = Time.time + 1f / _weaponConfig.FireRate; // Обновляем время следующего выстрела
            }
            
            else if (currentClip <= 0)
                _animancer.Play(EmptyClipAnim);
        }

        public async UniTask Recharge()
        {
            if (currentClip >= _weaponConfig.MagazineCapacity || totalAmmo == 0)
                return;

            _isReload = true;
            await RechargeProcess();
            await _animancer.Play(ReloadAnim).ToUniTask(cancellationToken: _cancellationTokenSource.Token);
            OnChangeAmmo?.Invoke(currentClip,WeaponConfig.RichTextAmmo,totalAmmo);
            _isReload = false;
        }
        
        protected virtual void ShootProcess()
        {
            //  _weaponBaseAnimations.ShowShoot(_animator);
            currentClip--;
            Debug.Log(currentClip);
            OnChangeAmmo?.Invoke(currentClip,WeaponConfig.RichTextAmmo,totalAmmo);
            
            Vector3 rayOrigin = _barrelTransform.position;
            Vector3 rayDirection = _barrelTransform.forward; 
            
            if (Physics.Raycast(rayOrigin, rayDirection,out var hit, _weaponConfig.Range, ~_characterLayerMask))
                HandleHit(hit);
            
        }
        
        protected abstract UniTask RechargeProcess();
        
        protected virtual void HandleHit(RaycastHit hit)
        {

            if (hit.transform.root.TryGetComponent(out HealthComponent healthComponent))
                healthComponent.TakeDamage(_weaponConfig.Damage);

            if (hit.transform.TryGetComponent(out Rigidbody rb))
                rb.AddForce(-hit.normal * _weaponConfig.ImpactForce);
        }
        private void OnDrawGizmos()
        {
            // Если у вас есть ссылка на ствол оружия
            if (_barrelTransform != null)
            {
                // Устанавливаем цвет Gizmos
                Gizmos.color = Color.red; // Цвет луча (красный)

                // Рисуем линию от ствола до направления луча
                Gizmos.DrawRay(_barrelTransform.position, _barrelTransform.forward * _weaponConfig.Range);
            }
        }
    }
}