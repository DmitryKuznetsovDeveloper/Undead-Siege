using Game.Components;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Conrollers
{
    public class CharacterMovementController : ITickable
    {
        private readonly IMoveInput _moveInput;
        private readonly IJumpInput _jumpInput;
        private readonly IMouseInput _mouseInput;
        private readonly MovementHandler _movementHandler;
        private readonly LookHandler _lookHandler;
        private bool _isGamepadActive;

        public CharacterMovementController(IMoveInput moveInput, IJumpInput jumpInput, IMouseInput mouseInput, MovementHandler movementHandler, LookHandler lookHandler)
        {
            _moveInput = moveInput;
            _jumpInput = jumpInput;
            _mouseInput = mouseInput;
            _movementHandler = movementHandler;
            _lookHandler = lookHandler;
        }

        public void Tick()
        {
            // Движение и прыжок
            _movementHandler.MoveCharacter(_moveInput.MoveDirection, _moveInput.IsRunButtonPressed, _jumpInput.IsJumpButtonPressed);
            
            // Определение активного устройства ввода
            _isGamepadActive = Mouse.current == null || Mouse.current.delta.ReadValue() == Vector2.zero;

            // Управление взглядом с учётом активного устройства
            _lookHandler.LookAround(_mouseInput.MouseDelta, _isGamepadActive);
        }
    }
}