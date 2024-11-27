﻿using Cysharp.Threading.Tasks;

namespace Game.Objects.Weapons
{
    public sealed class Gun : ShootingWeapon
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