using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


namespace MovementCamera
{
    public class MouseManager : InputManager
    {
        [SerializeField] private bool _moveScreenEdges = false;
        public static event MoveInputHandler OnMoveInput;
        public static event RotateInputHandler OnRotateInput;
        public static event ZoomInputHandler OnZoomInput;

        public static Action<Vector3> OnShiftInput;

        private Vector2Int _screen;
        private float _mousePositionOnRotateStart;

        private Plane _dragPlane;
        private Vector3 _mousePos;
        private Vector3 _difference;
        private Vector3 _origin;
        private Camera _camera;
        private bool drag = false;

        private void Awake()
        {
            _camera = Camera.main;
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

            
            MoveInputHandler_ScreenEdges(mousePosition);
            RotateInputHandler(mousePosition);
            ZoomInputHandler();
            MoveInputHandler_Drag();
        }

        private void LateUpdate()
        {
            //MoveInputHandler_Drag();
        }

        private void MoveInputHandler_Drag()
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    _mousePos = new Vector3(hit.point.x, 0 , hit.point.z);
                    _dragPlane = new Plane(Vector3.up, hit.point);
                    drag = true;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (!drag)
                {
                    return;
                }
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                float planeDistance;
                _dragPlane.Raycast(ray, out planeDistance);
                Vector3 newMousePos = ray.GetPoint(planeDistance);
                newMousePos = new Vector3(newMousePos.x, 0, newMousePos.z);

                Vector3 result = _mousePos - newMousePos;
                /// камера смещается на разницу расстояний, от точки нажатия, до места отведения курсора на плоскости
                /// после чего курсор может переместится за точку куда мы нажали, что сразу приведёт к тому что камера захочет переместится
                /// (из-за того что числа с плавающей точкой не совсем точные)
                /// в противополложное положение. Это приведёт к дёргнию. Чтобы избежать дёргания камеры разделим полученныё результат
                /// на небольшое число, это ласт плавности, правда при близком положение кусора будет заметно отстование.
                result = result / 1.1f;                    

                OnShiftInput?.Invoke(result);

                _mousePos = newMousePos;
                Debug.DrawRay(_camera.transform.position, ray.GetPoint(planeDistance), Color.red);
            }

            if (Input.GetMouseButtonUp(0))
            {
                drag = false;
            }
        }

        private void MoveInputHandler_ScreenEdges(Vector3 mousePosition)
        {
            if (!_moveScreenEdges) { return; }
            
            
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
