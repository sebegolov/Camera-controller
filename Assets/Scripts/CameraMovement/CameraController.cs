using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Test system
public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject _cameraTarget;
    [SerializeField] private GameObject _camera;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private float _normalSpeed;
    [SerializeField] private float _fastSpeed;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementTime;
    [SerializeField] private float _rotationAmount;
    [SerializeField] private Vector3 _zoomAmount;

    private Vector3 _newPosition;
    private Quaternion _newRotation;
    private Vector3 _newZoom;
    
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;

    void Start()
    {
        _newPosition = transform.position;
        _newRotation = transform.rotation;
        _newZoom = _cameraTransform.localPosition;
    }


    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray,out entry))
            {
                _dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray,out entry))
            {
                _dragCurrentPosition = ray.GetPoint(entry);

                _newPosition = transform.position + _dragStartPosition;
            }
        }
    }

    private void HandleMovementInput()
    {
        //float h = Input.GetAxis("Vertical");
        //Movement
        _movementSpeed = Input.GetKey(KeyCode.LeftShift) ? _fastSpeed : _normalSpeed;
        
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _newPosition += (transform.forward * _movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _newPosition += (-transform.forward * _movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _newPosition += (transform.right * _movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _newPosition += (-transform.right * _movementSpeed);
        }
        //Rotation
        if (Input.GetKey(KeyCode.Q))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * _rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            _newRotation *= Quaternion.Euler(Vector3.up * -_rotationAmount);
        }
        //Zoom
        if (Input.GetKey(KeyCode.R))
        {
            _newZoom += _zoomAmount;
        }
        if (Input.GetKey(KeyCode.R))
        {
            _newZoom -= _zoomAmount;
        }
        
        
        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.deltaTime * _movementTime);
        transform.position = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, Time.deltaTime * _movementTime);
    }
}
