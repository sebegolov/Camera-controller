using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MovementCamera
{
    public class MouseManager : InputManager
    {
        public static event MoveInputHandler OnMoveInput;
        public static event RotateInputHandler OnRotateInput;
        public static event ZoomInputHandler OnZoomInput;

        private Vector2Int _screen;
        private float _mousePositionOnRotateStart;

        private void Awake()
        {
            _screen = new Vector2Int(Screen.width, Screen.height);
        }

        private void OnApplicationFocus(bool focus)
        {
            _screen = new Vector2Int(Screen.width, Screen.height);
        }

        private void Update()
        {
            Vector3 mousePosition = Input.mousePosition;
            bool mouseValid = (mousePosition.y <= _screen.y * 1f &&
                               mousePosition.y >= _screen.y * -0f &&
                               mousePosition.x <= _screen.x * 1f &&
                               mousePosition.x >= _screen.x * -0f);

            if (!mouseValid) { return; }

            
            MoveInputHandler(mousePosition);
            RotateInputHandler(mousePosition);
            ZoomInputHandler();
        }

        private void MoveInputHandler(Vector3 mousePosition)
        {
            if (mousePosition.y > _screen.y * 0.95f)
            {
                OnMoveInput?.Invoke(Vector3.forward);
            }
            else if (mousePosition.y < _screen.y * 0.05f)
            {
                OnMoveInput?.Invoke(-Vector3.forward);
            }
            if (mousePosition.x > _screen.x * 0.95f)
            {
                OnMoveInput?.Invoke(Vector3.right);
            }
            else if (mousePosition.x < _screen.x * 0.05f)
            {
                OnMoveInput?.Invoke(-Vector3.right);
            }
        }
        
        private void RotateInputHandler(Vector3 mousePosition)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _mousePositionOnRotateStart = mousePosition.x;
            }else if (Input.GetMouseButton(1))
            {
                if (mousePosition.x < _mousePositionOnRotateStart)
                {
                    OnRotateInput?.Invoke(-1f);
                }
                else if(mousePosition.x > _mousePositionOnRotateStart)
                {
                    OnRotateInput?.Invoke(1f);
                }
            }
        }
        private void ZoomInputHandler()
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                OnZoomInput?.Invoke(-3f);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                OnZoomInput?.Invoke(3f);
            }
        }
    }
}
