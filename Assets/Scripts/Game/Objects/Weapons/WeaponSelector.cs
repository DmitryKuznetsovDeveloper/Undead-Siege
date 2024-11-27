using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Objects.Weapons
{
    public sealed class WeaponSelector
    {
        public event Action<ShootingWeapon> OnChangeWeapon;
        public ShootingWeapon CurrentShootingWeapon => _currentShootingWeapon;

        private int _currentWeaponIndex;
        private ShootingWeapon _currentShootingWeapon;
        private ShootingWeapon[] _weapons;

        public void Init(ShootingWeapon[] weapons)
        {
            if (weapons == null || weapons.Length == 0)
            {
                Debug.LogError("Weapon array is empty or null");
                return;
            }

            _weapons = weapons;
            _currentWeaponIndex = 0;
            _currentShootingWeapon = _weapons[_currentWeaponIndex];

            // Делаем активным только первое оружие, все остальные скрываем
            foreach (var weapon in _weapons)
                weapon.gameObject.SetActive(false);

            _currentShootingWeapon.gameObject.SetActive(true);
        }

        public void Next()
        {
            if (!_currentShootingWeapon.IsReload)
            {
                _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Length;
                _ = SelectWeapon();
            }
        }

        public void Previous()
        {
            if (!_currentShootingWeapon.IsReload)
            {
                _currentWeaponIndex = (_currentWeaponIndex - 1 + _weapons.Length) % _weapons.Length;
                _ = SelectWeapon();
            }
        }

        public UniTask GetWeaponAsync() => SelectWeapon();

        private async UniTask SelectWeapon()
        {
            if (_currentShootingWeapon != null)
            {
                await _currentShootingWeapon.HideWeapon();
                _currentShootingWeapon.Deactivate();
            }

            _currentShootingWeapon = _weapons[_currentWeaponIndex];
            _currentShootingWeapon.Activate();
            OnChangeWeapon?.Invoke(_currentShootingWeapon);
            await _currentShootingWeapon.ShowWeapon();
        }
    }
}
