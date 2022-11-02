using UnityEngine;

namespace MovementCamera
{
    public abstract class InputManager : MonoBehaviour
    {
        public delegate void MoveInputHandler(Vector3 moveVector);
        public delegate void RotateInputHandler(float moveVector);
        public delegate void ZoomInputHandler(float moveVector);
    }
}
