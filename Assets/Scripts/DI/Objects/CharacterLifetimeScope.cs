using System;
using System.Linq;
using Data.Character;
using Game.Components;
using Game.Controllers;
using Game.Input;
using Game.Objects.Weapons;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI.Objects
{
    public sealed class CharacterLifetimeScope : LifetimeScope
    {
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private CharacterMovementConfig _characterMovementConfig;
        [SerializeField] private MouseControlConfig _mouseControlConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            //Input
            builder.Register<IMoveInput, MoveInput>(Lifetime.Singleton).As<IInitializable>().As<ITickable>().As<IDisposable>();
            builder.Register<IJumpInput, JumpInput>(Lifetime.Singleton).As<IInitializable>().As<ITickable>().As<IDisposable>();;
            builder.Register<ILookInput, LookInput>(Lifetime.Singleton).As<IInitializable>().As<ITickable>().As<IDisposable>();;
            builder.Register<IPauseInput, PauseInput>(Lifetime.Singleton).As<IInitializable>().As<ITickable>().As<IDisposable>();;
            builder.Register<IWeaponInput, WeaponInput>(Lifetime.Singleton).As<IInitializable>().As<ITickable>().As<IDisposable>();;

            //Configs
            builder.RegisterInstance(_characterMovementConfig);
            builder.RegisterInstance(_mouseControlConfig);

            //Components
            builder.Register<MovementHandler>(Lifetime.Singleton).WithParameter(_bodyTransform).AsSelf();
            builder.Register<LookHandler>(Lifetime.Singleton).WithParameter(gameObject.transform).AsSelf();

            //Weapon
            builder.Register<WeaponSelector>(Lifetime.Singleton);
            builder.RegisterInstance(GetComponentsInChildren<ShootingWeapon>(true).ToArray());
            
            //Controllers
            builder.Register<CharacterMovementController>(Lifetime.Singleton).As<ITickable>();
            builder.Register<CharacterWeaponController>(Lifetime.Singleton).As<IInitializable>().As<ITickable>();

        }
    }
}