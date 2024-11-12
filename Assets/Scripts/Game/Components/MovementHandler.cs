using Data.Character;
using UnityEngine;

namespace Game.Components
{
    public sealed class MovementHandler
    {
        private readonly CharacterController _controller;
        private readonly CharacterMovementConfig _movementConfig;
        private readonly Transform _characterTransform;
        private float _verticalVelocity;
        private float _currentSpeed;

        public MovementHandler(CharacterController controller, CharacterMovementConfig movementConfig, Transform characterTransform)
        {
            _controller = controller;
            _movementConfig = movementConfig;
            _characterTransform = characterTransform;
        }

        public void MoveCharacter(Vector2 direction, bool isRun, bool isJump)
        {
            Vector3 move = _characterTransform.right * direction.x + _characterTransform.forward * direction.y;
            
            if (_controller.isGrounded)
            {
                if (_verticalVelocity < 0)
                    _verticalVelocity = -1f; // Слегка отрицательное значение для надёжного контакта с землёй
                
                if (isJump)
                    _verticalVelocity = _movementConfig.JumpForce;
            }
            else
            {
                _verticalVelocity += _movementConfig.Gravity * Time.deltaTime;
            }
            move.y = _verticalVelocity;

            // Плавное изменение скорости при переходе между ходьбой и бегом
            var targetSpeed = isRun ? _movementConfig.RunSpeed : _movementConfig.MoveSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _movementConfig.Acceleration * Time.deltaTime);
            _controller.Move(move * (_currentSpeed * Time.deltaTime));
        }
    }
}
