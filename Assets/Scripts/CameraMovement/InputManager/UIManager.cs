using System;
using UnityEngine;
using UnityEngine.UI;

namespace MovementCamera
{
    public class UIManager : InputManager
    {
        public static event MoveInputHandler OnMoveInput;
        public static event RotateInputHandler OnRotateInput;
        public static event ZoomInputHandler OnZoomInput;

        [SerializeField] private float _retreat = 10;
        [SerializeField] private RectTransform _workField;

        private Vector3[] _corners = new Vector3[4];

        private void Awake()
        {
            _workField.GetWorldCorners(_corners);
        }
        
        private void Update()
        {
            Vector3 mousePosition = Input.mousePosition;
            bool mouseValid = (mousePosition.y <= _corners[1].y &&
                               mousePosition.y >= _corners[3].y &&
                               mousePosition.x <= _corners[3].x &&
                               mousePosition.x >= _corners[1].x);

            if (!mouseValid) { return; }

            
            MoveInputHandler(mousePosition);
        }

        private void MoveInputHandler(Vector3 mousePosition)
        {
            if (mousePosition.y > _corners[1].y - _retreat)
            {
                OnMoveInput?.Invoke(Vector3.forward);
            }
            else if (mousePosition.y < _corners[3].y + _retreat)
            {
                OnMoveInput?.Invoke(-Vector3.forward);
            }
            if (mousePosition.x > _corners[3].x - _retreat)
            {
                OnMoveInput?.Invoke(Vector3.right);
            }
            else if (mousePosition.x < _corners[1].x + _retreat)
            {
                OnMoveInput?.Invoke(-Vector3.right);
            }
        }

    }
}
