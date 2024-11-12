using System;
using UnityEngine.InputSystem;
using VContainer.Unity;
namespace Game.Input
{
    public interface IWeaponInput
    {
        public bool IsShootButtonPressed { get; }
        public bool IsAimButtonPressed { get; }
        public bool IsReloadButtonPressed { get; }
    }
    public sealed class WeaponInput : IWeaponInput, IInitializable, ITickable, IDisposable
    {
        private InputAction _shootAction;
        private InputAction _aimAction;
        private InputAction _reloadAction;
        public bool IsShootButtonPressed { get; private set; }
        public bool IsAimButtonPressed { get; private set; }
        public bool IsReloadButtonPressed { get; private set; }

        void IInitializable.Initialize()
        {
            _shootAction = new InputAction("shoot");
            _shootAction.AddBinding("<Mouse>/leftButton");
            _shootAction.AddBinding("<Gamepad>/rightTrigger");
            _shootAction.Enable();

            _aimAction = new InputAction("aim");
            _aimAction.AddBinding("<Mouse>/rightButton");
            _aimAction.AddBinding("<Gamepad>/leftTrigger");
            _aimAction.Enable();

            _reloadAction = new InputAction("reload");
            _reloadAction.AddBinding("<Keyboard>/R");
            _reloadAction.AddBinding("<Gamepad>/buttonWest");
            _reloadAction.Enable();
        }

        void ITickable.Tick()
        {
            IsShootButtonPressed = _shootAction.IsPressed();
            IsAimButtonPressed = _aimAction.IsPressed();
            IsReloadButtonPressed = _reloadAction.WasPressedThisFrame();
        }

        void IDisposable.Dispose()
        {
            _shootAction?.Dispose();
            _aimAction?.Dispose();
            _reloadAction?.Dispose();
        }
    }
}