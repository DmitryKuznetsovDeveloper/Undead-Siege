using Cysharp.Threading.Tasks;
using Game.Objects.Weapons;
namespace Game.Weapon
{
    public class Scar : ShootingWeapon
    {
        protected override async UniTask RechargeProcess()
        {
            int ammoNeeded = _weaponConfig.MagazineCapacity - currentClip;
            if (totalAmmo >= ammoNeeded)
            {
                totalAmmo -= ammoNeeded;
                currentClip = _weaponConfig.MagazineCapacity;
            }
            else
            {
                currentClip += totalAmmo;
                totalAmmo = 0;
            }
            await UniTask.CompletedTask;
        }
    }
}