using System;
using UnityEngine;

namespace MovementCamera
{
    public class KeyboardManager : InputManager
    {
        public static event MoveInputHandler OnMoveInput;
        public static event RotateInputHandler OnRotateInput;
        public static event ZoomInputHandler OnZoomInput;

        public static Action OnSpeedScale;

        private void Update()
        {
            MoveInputHandler();
            RotateInputHandler();
            ZoomInputHandler();
            SpeedScale();
        }

        private void SpeedScale()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                OnSpeedScale?.Invoke();
            }
        }

        private void MoveInputHandler()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                OnMoveInput?.Invoke(Vector3.forward);
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                OnMoveInput?.Invoke(-Vector3.forward);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                OnMoveInput?.Invoke(Vector3.right);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                OnMoveInput?.Invoke(-Vector3.right);
            }
        }

        private void RotateInputHandler()
        {
            if (Input.GetKey(KeyCode.E))
            {
                OnRotateInput?.Invoke(new Vector2(-1f, 0));
            }
            if (Input.GetKey(KeyCode.Q))
            {
                OnRotateInput?.Invoke(new Vector2(1f,0));
            }
        }

        private void ZoomInputHandler()
        {
            if (Input.GetKey(KeyCode.Z))
            {
                OnZoomInput?.Invoke(-1f);
            }
            if (Input.GetKey(KeyCode.X))
            {
                OnZoomInput?.Invoke(1f);
            }
        }
    }
}
