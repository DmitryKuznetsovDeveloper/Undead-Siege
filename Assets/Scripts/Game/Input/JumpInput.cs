using System;
using UnityEngine.InputSystem;
using VContainer.Unity;
namespace Game.Input
{
    public sealed class JumpInput : IInitializable, ITickable, IDisposable
    {
        private InputAction _jumpAction;
        public bool IsJumpButtonPressed { get;private set; }

        void IInitializable.Initialize()
        {
            _jumpAction = new InputAction("jump");
            _jumpAction.AddBinding("<Keyboard>/space");         
            _jumpAction.AddBinding("<Gamepad>/buttonSouth"); 
            _jumpAction.Enable();
        }

        void ITickable.Tick() => IsJumpButtonPressed = _jumpAction.WasPressedThisFrame();

        void IDisposable.Dispose() => _jumpAction?.Dispose();
    }
}