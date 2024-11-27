﻿using System;
using System.Threading;
using Animancer;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Data.Character.WeaponsConfigs;
using Game.Components;
using Game.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Objects.Weapons
{
    public abstract class ShootingWeapon : MeleeWeapon
    {
        public event Action<int,string,int> OnChangeAmmo;
        public bool IsReload => _isReload;

        public ShootingWeaponConfig WeaponConfig => _weaponConfig;
        public CinemachineVirtualCamera BaseCamera => _baseCamera;
        
        [SerializeField] private Transform _barrelTransform;
        [SerializeField] protected Transform _muzzleFlashContainer;
        [SerializeField] protected Transform _bloodContainer;
        [SerializeField] protected Transform _impactContainer;
        [SerializeField] protected Transform _shellContainer;
        [SerializeField] protected Transform _holeContainer;
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
        
        private Pool.ObjectPool<ParticleSystem> _muzzleFlashPool; // Пул для вспышек выстрела
        private Pool.ObjectPool<ParticleSystem> _bloodEffectPool; // Пул для эффектов крови
        private Pool.ObjectPool<ParticleSystem> _impactEffectPool; // Пул для эффектов попаданий
        private Pool.ObjectPool<ParticleSystem> _shellEffectPool; // Пул для эффектов попаданий
        private Pool.ObjectPool<ParticleSystem> _holePool; // Пул для эффектов попаданий// Камера игрока

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
            _muzzleFlashPool = new Pool.ObjectPool<ParticleSystem>(_weaponConfig.MuzzleFlashEffect, _weaponConfig.MagazineCapacity / 2, _muzzleFlashContainer);
            _bloodEffectPool = new Pool.ObjectPool<ParticleSystem>(_weaponConfig.BloodEffect, _weaponConfig.MagazineCapacity / 2, _bloodContainer);
            _impactEffectPool = new Pool.ObjectPool<ParticleSystem>(_weaponConfig.SurfaceImpactEffect, _weaponConfig.MagazineCapacity / 2, _impactContainer);
            _shellEffectPool = new Pool.ObjectPool<ParticleSystem>(_weaponConfig.ShellEjectionEffect, _weaponConfig.MagazineCapacity / 2, _shellContainer);
            _holePool = new Pool.ObjectPool<ParticleSystem>(_weaponConfig.BulletHoleDecal, _weaponConfig.MagazineCapacity, _holeContainer);
        }
        
        public void Activate() => gameObject.SetActive(true);
        public void Deactivate() => gameObject.SetActive(false);
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
            var shootState = _animancer.Play(ShootAnim);
            PlayEffectParent(_muzzleFlashPool, _muzzleFlashContainer);
            shootState.Events(this).OnEnd = () => _animancer.Play(IdleAnim);
            currentClip--;
            Debug.Log(currentClip);
            OnChangeAmmo?.Invoke(currentClip,WeaponConfig.RichTextAmmo,totalAmmo);
            
            Vector3 rayOrigin = _barrelTransform.position;
            Vector3 rayDirection = _barrelTransform.forward; 
            
            if (Physics.Raycast(rayOrigin, rayDirection,out var hit, _weaponConfig.Range, ~_characterLayerMask))
                HandleHit(hit);
            
            PlayEffectParent(_shellEffectPool, _shellContainer);
        }
        
        protected abstract UniTask RechargeProcess();
        
        protected virtual void HandleHit(RaycastHit hit)
        {
            if (hit.transform.root.TryGetComponent(out HealthComponent healthComponent))
            {
                healthComponent.TakeDamage(_weaponConfig.Damage);
                PlayEffectHit(_bloodEffectPool, hit);
            }
            else
            {
                PlayEffectHit(_impactEffectPool, hit);
                PlayEffectAndAssignParent(_holePool, hit);
            }

            if (hit.transform.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(-hit.normal * _weaponConfig.ImpactForce);
            }
        }
        
        private void PlayEffectHit(Pool.ObjectPool<ParticleSystem> pool, RaycastHit hit)
        {
            var poolObject = pool.GetObject();
            var poolObjectTransform = poolObject.transform;
            poolObjectTransform.position = hit.point;
            poolObjectTransform.rotation = Quaternion.LookRotation(hit.normal);
            poolObject.Play(true);
            ReturnEffectToPoolAfterTime(pool, poolObject).Forget();
        }

        private void PlayEffectParent(Pool.ObjectPool<ParticleSystem> pool, Transform parent)
        {
            var poolObject = pool.GetObject();
            var poolObjectTransform = poolObject.transform;
            poolObjectTransform.position = parent.position;
            poolObjectTransform.rotation = parent.rotation;
            poolObject.Play(true);
            ReturnEffectToPoolAfterTime(pool, poolObject).Forget();
        }

        private void PlayEffectAndAssignParent(Pool.ObjectPool<ParticleSystem> pool, RaycastHit hit)
        {
            var poolObject = pool.GetObject();
            var poolObjectTransform = poolObject.transform;
            poolObjectTransform.SetParent(hit.transform);
            poolObjectTransform.position = hit.point;
            poolObjectTransform.rotation = Quaternion.LookRotation(hit.normal);
            poolObject.Play(true);
            ReturnEffectToPoolAfterTime(pool, poolObject).Forget();
        }

        private async UniTask ReturnEffectToPoolAfterTime(Pool.ObjectPool<ParticleSystem> pool, ParticleSystem effect)
        {
            // Ожидаем, пока эффект не завершит воспроизведение
            while (effect.isPlaying)
                await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token); // Ждем следующий кадр

            effect.transform.SetParent(pool.Parent);
            pool.ReturnObject(effect);
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