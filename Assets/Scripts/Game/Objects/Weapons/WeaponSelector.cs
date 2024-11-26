using System;
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
            for (var i = 0; i < _weapons.Length; i++)
                _weapons[i].gameObject.SetActive(i == _currentWeaponIndex);
        }

        public void Next()
        {
            if (!_currentShootingWeapon.IsReload)
            {
                _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Length; // Обрабатываем переполнение
                SelectWeapon();
            }
        }

        public void Previous()
        {
            if (!_currentShootingWeapon.IsReload)
            {
                _currentWeaponIndex = (_currentWeaponIndex - 1 + _weapons.Length) % _weapons.Length; // Обрабатываем отрицательные значения индекса
                SelectWeapon();
            }
        }

        public void GetWeapon() => SelectWeapon();

        private async void SelectWeapon()
        {
            if (_currentShootingWeapon != null)
            {
                await _currentShootingWeapon.HideWeapon();
                _currentShootingWeapon.gameObject.SetActive(false);
            }

            _currentShootingWeapon = _weapons[_currentWeaponIndex];
            _currentShootingWeapon.gameObject.SetActive(true);
            OnChangeWeapon?.Invoke(_currentShootingWeapon);
            await _currentShootingWeapon.ShowWeapon();
        }
    }
}