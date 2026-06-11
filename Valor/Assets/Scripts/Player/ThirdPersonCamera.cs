using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.XR;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Valor.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target")] 
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 focusOffset = new Vector3(0f, 1.5f, 0f);
        [SerializeField] private float shoulderOffset = 0.6f;

        [Header("Orbit")] 
        [SerializeField] private float distance = 4f;
        [SerializeField] private float sensitivity = 2.5f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 70f;

        private float yaw;
        private float pitch;
        
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;
        }

        // Update is called once per frame
        private void Update()
        {
            ReadMouseLook();
            HandleCursorUnlock();
        }

        private void LateUpdate()
        {
            PositionCamera();
        }

        private void ReadMouseLook()
        {
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        private void PositionCamera()
        {
            Vector3 focusPoint = target.position + focusOffset;
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 position = focusPoint + rotation * Vector3.back * distance
                                          + rotation * Vector3.right * shoulderOffset;
            transform.SetPositionAndRotation(position, rotation);
        }

        private void HandleCursorUnlock()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Cursor.lockState = CursorLockMode.None;
        }
    }
}