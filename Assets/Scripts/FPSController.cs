
using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class FPSController : MonoBehaviour
{
    private CharacterController controller;

    //Inputs
    private InputAction moveAction;

    private Vector2 moveInput;

    private InputAction aimAction;

    private InputAction jumpAction;

    [SerializeField] private float movementSpeed = 5;

    [SerializeField] private float jumpHeight = 2;

    [SerializeField] private float gravity = -9.81f;

    [SerializeField] Transform sensor;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] float sensorRadius;

    private Vector3 playerGravity;

    private InputAction lookAction;
    [SerializeField] private Vector2 lookInput;

    [SerializeField] private float cameraSensitivity = 10;

    [SerializeField] Transform lookAtCamera;

    float xRotation;

    private Transform mainCamera;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;

        moveAction = InputSystem.actions["Move"];
        jumpAction = InputSystem.actions["Jump"];
        aimAction = InputSystem.actions["Aim"];
        lookAction = InputSystem.actions["Look"];
    
    }

    
    void Update()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        
        moveInput = moveAction.ReadValue<Vector2>();

        Movement();

         if (jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        Gravity();
    }

    void Movement()
    {

        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

       
        float mouseX = lookInput.x * cameraSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * cameraSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.Rotate(Vector3.up, mouseX);
        lookAtCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //lookAtCamera.Rotate(Vector3.right, mouseY);

        if(direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            controller.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
    }


    void Gravity() // Para cambiar el valor de la gravedad
    {
        if (!IsGrounded())
        {
            playerGravity.y += gravity * Time.deltaTime;



        }
        else if (IsGrounded() && playerGravity.y < -2)
        {
            playerGravity.y = gravity;
        }
        
        controller.Move(playerGravity * Time.deltaTime);

    }

    void Jump()
    {
        playerGravity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        controller.Move(playerGravity * Time.deltaTime);
    }

    bool IsGrounded()
    {
        
        return Physics.CheckSphere(sensor.position, sensorRadius, groundLayer);
    }
    void OnDrawGizmos() // Para crear el radio del ground sensor
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(sensor.position, sensorRadius);
    }
}
