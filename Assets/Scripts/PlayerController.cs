
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //Controller
    private CharacterController controller;

    //Inputs
    private InputAction moveAction;

    private Vector2 moveInput;

    [SerializeField] private float movementSpeed = 5;

    //Gravedad

    [SerializeField] private float gravity = -9.81f;

    private Vector3 playerGravity;

    //Ground Sensor

    [SerializeField] Transform sensor;

    [SerializeField] LayerMask groundLayer;

    [SerializeField] float sensorRadius;

    //Salto 


    [SerializeField] private float jumpHeight = 2;

    private InputAction jumpAction;

    [SerializeField] private float smoothTime = 0.2f;

    private float turnSmoothVelocity;

    private InputAction lookAction;
    private Vector2 lookInput;

    private Transform mainCamera;

    private InputAction aimAction;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions["Move"];
        jumpAction = InputSystem.actions["Jump"];
        lookAction = InputSystem.actions["Look"];
        aimAction = InputSystem.actions["Aim"];
        mainCamera = Camera.main.transform;

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()

    {
        lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();

        //Movimiento2();

        //MovimientoCutre();

        if (aimAction.IsInProgress())
        {
            AimMovement();
        }
        else
        {
            Movement();
        }

        if (jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
        }

        Gravity();

        if(aimAction.WasPerformedThisFrame())
        
        {
            Attack();
        }

    
    
    }

    void Movement() // Movimiento de camara hecho de una mejor forma
    {
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);
        
        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;


            controller.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
        }

    }
    void AimMovement() // Al pulsar click derecho se aplica este movimiento
    {
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);


        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, mainCamera.eulerAngles.y, ref turnSmoothVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
        
        if (direction != Vector3.zero)
        {

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
        }
    }

    void Movimiento2() //Movimiento que sigue al raton como en el hades
    {
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        Ray ray = Camera.main.ScreenPointToRay(lookInput);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 playerForward = hit.point - transform.position;
            Debug.Log(hit.transform.name);
            playerForward.y = 0;
            transform.forward = playerForward;
        }

        if (direction != Vector3.zero)
            {
                controller.Move(direction.normalized * movementSpeed * Time.deltaTime);
            }
    }
    void MovimientoCutre() // Pochedumbre
    {


        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);


            controller.Move(direction.normalized * movementSpeed * Time.deltaTime);
        }


    }
    
    void Attack()
    {
         Ray ray = Camera.main.ScreenPointToRay(lookInput);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage();
            }
            
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
