using Cysharp.Threading.Tasks;
using Game.Input;
using Game.Objects.Weapons;
using VContainer.Unity;

namespace Game.Controllers
{
    public sealed class CharacterWeaponController : IInitializable, ITickable
    {
        private readonly ShootingWeapon[] _weapons;
        private readonly WeaponSelector _weaponSelector;
        private readonly IWeaponInput _weaponInput;
        private readonly ILookInput _lookInput;
        
        public CharacterWeaponController(ShootingWeapon[] weapons, WeaponSelector weaponSelector, IWeaponInput weaponInput, ILookInput lookInput)
        {
            _weapons = weapons;
            _weaponSelector = weaponSelector;
            _weaponInput = weaponInput;
            _lookInput = lookInput;
        }
        
        void IInitializable.Initialize()
        {
            _weaponSelector.Init(_weapons);
            _weaponSelector.GetWeapon();
        }
        public async void Tick()
        {
           await AsyncRecharge();

            SelectWeapon();

            Shoot();

            MeleeAttack();

            ChangeAimCamera();
        }
        private void ChangeAimCamera() => _weaponSelector.CurrentShootingWeapon.BaseCamera.gameObject.SetActive(!_weaponInput.IsAimButtonPressed);
        private void SelectWeapon()
        {
            if (_lookInput.MouseScroll.y >= 0.1f || _lookInput.IsTriangleButtonPressed)
                _weaponSelector.Next();

            if (_lookInput.MouseScroll.y <= -0.1f)
                _weaponSelector.Previous();
        }
        private void Shoot()
        {
            if (_weaponInput.IsShootButtonPressed)
                _weaponSelector.CurrentShootingWeapon.Shoot();
        }
        
        private void MeleeAttack()
        {
            if (_weaponInput.IsMeleeAttackButtonPressed)
                _weaponSelector.CurrentShootingWeapon.MeleeAttackAnimation();
        }
        private async UniTask AsyncRecharge()
        {
            if (_weaponInput.IsReloadButtonPressed && _weaponSelector.CurrentShootingWeapon)
                await _weaponSelector.CurrentShootingWeapon.Recharge();
        }
    }
}