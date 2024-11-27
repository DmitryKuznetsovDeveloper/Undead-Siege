using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Input
{

    public interface ILookInput
    {
        public float2 LookDelta { get; }
        public float2 MouseScroll { get; }
        public bool IsTriangleButtonPressed { get; }
        public bool IsGamepadInput { get; }
    }
    public sealed class LookInput : ILookInput, IInitializable, ITickable, IDisposable
    {
        private InputAction _lookAction;
        private InputAction _mouseScrollAction;
        private InputAction _triangleButtonAction;

        public float2 LookDelta { get; private set; }
        public float2 MouseScroll { get; private set; }
        public bool IsTriangleButtonPressed { get; private set; }

        public bool IsGamepadInput { get; private set; }

        public void Initialize()
        {
            _lookAction = new InputAction("look");
            _lookAction.AddBinding("<Mouse>/delta");
            _lookAction.AddBinding("<Gamepad>/rightStick");

            _lookAction.started += context => LookDelta = context.ReadValue<Vector2>();
            _lookAction.performed += context => LookDelta = context.ReadValue<Vector2>();
            _lookAction.canceled += context => LookDelta = context.ReadValue<Vector2>();
            _lookAction.Enable();


            _mouseScrollAction = new InputAction("scroll", binding: "<Mouse>/scroll");
            _mouseScrollAction.started += context => MouseScroll = context.ReadValue<Vector2>();
            _mouseScrollAction.performed += context => MouseScroll = context.ReadValue<Vector2>();
            _mouseScrollAction.canceled += context => MouseScroll = context.ReadValue<Vector2>();
            _mouseScrollAction.Enable();

            _triangleButtonAction = new InputAction("triangleButton", binding: "<Gamepad>/buttonNorth");
            _triangleButtonAction.Enable();
        }

        void ITickable.Tick()
        {
            IsTriangleButtonPressed = _triangleButtonAction.WasPerformedThisFrame();
            IsGamepadInput = _lookAction.activeControl?.device is Gamepad;
        }

        public void Dispose()
        {
            _lookAction?.Dispose();
            _mouseScrollAction?.Dispose();
        }
    }
}