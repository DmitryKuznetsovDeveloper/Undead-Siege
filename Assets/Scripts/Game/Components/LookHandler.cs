using Data.Character;
using UnityEngine;

namespace Game.Components
{
    public sealed class LookHandler
    {
        private readonly MouseControlConfig _mouseConfig;
        private readonly Transform _characterBody;
        private const float DeadZone = 0.2f;
        private float _rotationY;
        private float _rotationX;

        public LookHandler(MouseControlConfig mouseConfig, Transform characterBody)
        {
            _mouseConfig = mouseConfig;
            _characterBody = characterBody;
            _rotationX = _characterBody.eulerAngles.y; // Инициализация текущего угла поворота по Y
        }

        public void LookAround(Vector2 input, bool isGamepad)
        {
            if (input.sqrMagnitude < DeadZone * DeadZone) return;

            // Определение чувствительности
            float sensitivityX = isGamepad ? _mouseConfig.GamepadSensitivityX : _mouseConfig.MouseSensitivityX;
            float sensitivityY = isGamepad ? _mouseConfig.GamepadSensitivityY : _mouseConfig.MouseSensitivityY;

            // Накопление поворота от ввода
            _rotationX += input.x * (sensitivityX * Time.deltaTime);
            _rotationY -= input.y * (sensitivityY * Time.deltaTime);

            // Ограничение угла поворота по вертикали
            _rotationY = Mathf.Clamp(_rotationY, _mouseConfig.MinVerticalAngle, _mouseConfig.MaxVerticalAngle);

            // Применение поворота по осям отдельно для избегания скачков
            _characterBody.localRotation = Quaternion.Euler(_rotationY, _rotationX, 0f);
        }
    }
}