using Game.Components;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Controllers
{
    public class CharacterMovementController : ITickable
    {
        private readonly IMoveInput _moveInput;
        private readonly IJumpInput _jumpInput;
        private readonly ILookInput _lookInput;
        private readonly MovementHandler _movementHandler;
        private readonly LookHandler _lookHandler;
        private bool _isGamepadActive;

        public CharacterMovementController(IMoveInput moveInput, IJumpInput jumpInput, ILookInput lookInput, MovementHandler movementHandler, LookHandler lookHandler)
        {
            _moveInput = moveInput;
            _jumpInput = jumpInput;
            _lookInput = lookInput;
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
            _lookHandler.LookAround(_lookInput.MouseDelta, _isGamepadActive);
        }
    }
}