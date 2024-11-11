using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Game.Input
{
    public sealed class MoveInput : IInitializable,ITickable,IDisposable
    {
        private InputAction _moveAction;
        private InputAction _runAction;
        public float2 MoveDirection { get; private set; }
        public bool IsRunButtonPressed  { get; private set; }

        void IInitializable.Initialize()
        {
            _moveAction = new InputAction("move");
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d")
                .With("Up", "<Gamepad>/dpad/up")
                .With("Down", "<Gamepad>/dpad/down")
                .With("Left", "<Gamepad>/dpad/left")
                .With("Right", "<Gamepad>/dpad/right")
                .With("Up", "<Gamepad>/leftStick/up")
                .With("Down", "<Gamepad>/leftStick/down")
                .With("Left", "<Gamepad>/leftStick/left")
                .With("Right", "<Gamepad>/leftStick/right");

            _moveAction.started += context => { MoveDirection = context.ReadValue<Vector2>(); };
            _moveAction.performed += context => { MoveDirection = context.ReadValue<Vector2>(); };
            _moveAction.canceled += context => { MoveDirection = context.ReadValue<Vector2>(); };
            _moveAction.Enable();

            _runAction = new InputAction("run");
            _runAction.AddBinding("<Keyboard>/leftShift");        
            _runAction.AddBinding("<Gamepad>/leftStickPress"); 
            _runAction.Enable();
            
            _runAction.Enable();
        }
        
        void ITickable.Tick() => IsRunButtonPressed = _runAction.IsPressed();

        void IDisposable.Dispose() => _moveAction?.Dispose();
    }
}