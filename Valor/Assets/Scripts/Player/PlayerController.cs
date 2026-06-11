using System;
using UnityEngine;

namespace Valor.Player
{
    [RequireComponent((typeof(CharacterController)))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotationSpeed;
        
        [Header("Gravity & Jump")]
        [SerializeField] private float gravity;
        [SerializeField] private float jumpHeight;

        private const float GroundedStickForce = -2f;
        private const float MoveDeadzone = 0.001f;
        
        private CharacterController controller;
        private Transform cam;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cam = Camera.main.transform;
        }

        private void Update()
        {
            Vector3 moveDir = GetCameraRelativeInput();
            RotateTowardMovement(moveDir);
            ApplyGravitationAndJump();
            MoveCharacter(moveDir);
        }

        private Vector3 GetCameraRelativeInput()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            
            // Clamp so diagonals aren't faster than cardinals,
            // while stile allowing partial/analog input below full tilt.
            Vector3 input = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1f);

            // Flatten the camera's axes onto the ground plane: when the camera
            // tilts down, it's forward points partly into the floor
            Vector3 camForward = cam.forward;
            Vector3 camRight = cam.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Move relative to where the camera faces, not world axes.
            return camForward * input.z + camRight * input.x;
        }

        private void RotateTowardMovement(Vector3 moveDir)
        {
            if (moveDir.sqrMagnitude < MoveDeadzone)
                return;
           
            Quaternion target = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        private void ApplyGravitationAndJump()
        {
            if (controller.isGrounded)
            {
                verticalVelocity = GroundedStickForce;
                if (Input.GetButtonDown("Jump"))
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private void MoveCharacter(Vector3 moveDir)
        {
            Vector3 velocity = moveDir * moveSpeed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
