using System;
using UnityEngine.InputSystem;
using VContainer.Unity;
namespace Game.Input
{
    public interface IPauseInput
    {
        public bool IsPauseButtonPressed { get; }
    }
    public sealed class PauseInput : IPauseInput, IInitializable, ITickable, IDisposable
    {
        private InputAction _pauseAction;
        public bool IsPauseButtonPressed { get; private set; }

        void IInitializable.Initialize()
        {
            _pauseAction = new InputAction("pause");
            _pauseAction.AddBinding("<Keyboard>/escape");
            _pauseAction.AddBinding("<Gamepad>/start");
            _pauseAction.AddBinding("<Gamepad>/touchpadButton");
            _pauseAction.AddBinding("<Gamepad>/menu");
            _pauseAction.AddBinding("<Gamepad>/select");
            _pauseAction.Enable();
        }

        void ITickable.Tick() => IsPauseButtonPressed = _pauseAction.WasPressedThisFrame();

        void IDisposable.Dispose() => _pauseAction?.Dispose();
    }
}