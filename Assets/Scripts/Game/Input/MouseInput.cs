using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Input
{

    public interface IMouseInput
    {
        public float2 MouseDelta { get; }
        public float2 MouseScroll { get; }
        public bool IsTriangleButtonPressed { get; }
    }
    public sealed class MouseInput : IMouseInput, IInitializable, ITickable, IDisposable
    {
        private InputAction _lookAction;
        private InputAction _mouseScrollAction;
        private InputAction _triangleButtonAction;

        public float2 MouseDelta { get; private set; }
        public float2 MouseScroll { get; private set; }
        public bool IsTriangleButtonPressed { get; private set; }

        public void Initialize()
        {
            _lookAction = new InputAction("look");
            _lookAction.AddBinding("<Mouse>/delta");
            _lookAction.AddBinding("<Gamepad>/rightStick");

            _lookAction.started += context => MouseDelta = context.ReadValue<Vector2>();
            _lookAction.performed += context => MouseDelta = context.ReadValue<Vector2>();
            _lookAction.canceled += context => MouseDelta = context.ReadValue<Vector2>();
            _lookAction.Enable();


            _mouseScrollAction = new InputAction("scroll", binding: "<Mouse>/scroll");
            _mouseScrollAction.started += context => MouseScroll = context.ReadValue<Vector2>();
            _mouseScrollAction.performed += context => MouseScroll = context.ReadValue<Vector2>();
            _mouseScrollAction.canceled += context => MouseScroll = context.ReadValue<Vector2>();
            _mouseScrollAction.Enable();

            _triangleButtonAction = new InputAction("triangleButton", binding: "<Gamepad>/buttonNorth");
            _triangleButtonAction.Enable();
        }

        void ITickable.Tick() => IsTriangleButtonPressed = _triangleButtonAction.WasPerformedThisFrame();

        public void Dispose()
        {
            _lookAction?.Dispose();
            _mouseScrollAction?.Dispose();
        }
    }
}