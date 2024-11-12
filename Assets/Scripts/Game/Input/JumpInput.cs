using System;
using UnityEngine.InputSystem;
using VContainer.Unity;
namespace Game.Input
{
    public interface IJumpInput
    {
        public bool IsJumpButtonPressed { get; }
    }
    public sealed class JumpInput : IJumpInput, IInitializable, ITickable, IDisposable
    {
        private InputAction _jumpAction;
        public bool IsJumpButtonPressed { get; private set; }

        void IInitializable.Initialize()
        {
            _jumpAction = new InputAction("jump");
            _jumpAction.AddBinding("<Keyboard>/space");
            _jumpAction.AddBinding("<Gamepad>/buttonSouth");
            _jumpAction.Enable();
        }

        void ITickable.Tick() => IsJumpButtonPressed = _jumpAction.IsPressed();

        void IDisposable.Dispose() => _jumpAction?.Dispose();
    }
}