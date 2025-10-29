
using Unity.VisualScripting;
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

    [SerializeField] private float pushForce = 10;

    [SerializeField] private float throwForce = 20;

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

    private InputAction grabAction;

    private InputAction throwAction;

    //Coger objetos

    [SerializeField] private Transform Manos;
    [SerializeField] private Transform grabedObject;

    [SerializeField] private Vector3 handSensorSize;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions["Move"];
        jumpAction = InputSystem.actions["Jump"];
        lookAction = InputSystem.actions["Look"];
        aimAction = InputSystem.actions["Aim"];
        mainCamera = Camera.main.transform;
        grabAction = InputSystem.actions["Interact"];
        throwAction = InputSystem.actions["Throw"];

    }

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

        if (aimAction.WasPerformedThisFrame())

        {
            Attack();
        }

        if (grabAction.WasPerformedThisFrame())
        {
            GrabObject();
        }
        if (throwAction.WasPerformedThisFrame())
        {
            Throw();
        }

        RayTest();

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
                damageable.TakeDamage(5);
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

    /*bool IsGrounded()
    {

        return Physics.CheckSphere(sensor.position, sensorRadius, groundLayer);
    }*/

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(sensor.position, -transform.up, sensorRadius, groundLayer))
        {
            Debug.DrawRay(sensor.position, -transform.up * sensorRadius, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(sensor.position, -transform.up * sensorRadius, Color.green);
            return false;
        }
    }
    void OnDrawGizmos() // Para crear el radio del ground sensor
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(sensor.position, sensorRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Manos.position, handSensorSize);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.gameObject.tag == "Empujable")
        {
            // Las dos funcionan igual
            Rigidbody rBody = hit.collider.attachedRigidbody;
            //Rigidbody rBody = hit.transform.GetComponent<Rigidbody>();

            if (rBody == null || rBody.isKinematic)
            {
                return;
            }

            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            rBody.linearVelocity = pushDirection * pushForce / rBody.mass;

        }
    }

    void GrabObject()
    {
        if (grabedObject == null)
        {
            Collider[] objectsToGrab = Physics.OverlapBox(Manos.position, handSensorSize);

            foreach (Collider item in objectsToGrab)
            {
                IGrabeable grabeableObject = item.GetComponent<IGrabeable>();

                if (grabeableObject != null)
                {
                    grabedObject = item.transform;
                    grabedObject.SetParent(Manos);
                    grabedObject.position = Manos.position;
                    grabedObject.rotation = Manos.rotation;
                    grabedObject.GetComponent<Rigidbody>().isKinematic = true;

                    return;
                }
            }
        }
        else
        {

            grabedObject.SetParent(null);
            grabedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabedObject = null;

        }

    }

    void Throw()
    {
        if (grabedObject == null)
        {
            return;
        }

        Rigidbody grabedBody = grabedObject.GetComponent<Rigidbody>();

        grabedObject.SetParent(null);
        grabedBody.isKinematic = false;
        grabedBody.AddForce(mainCamera.transform.forward * throwForce, ForceMode.Impulse);
        grabedObject = null;
    }

    void RayTest()
    {
        //Raycast smple

        if (Physics.Raycast(transform.position, transform.forward, 5))
        {
            Debug.Log("Hit");
            Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
        {
            Debug.Log(hit.transform.name);
            Debug.Log(hit.transform.position);
            Debug.Log(hit.transform.gameObject.layer);
            Debug.Log(hit.transform.tag);


            /*if(hit.transform.tag == "Empujable")
              {
                 Box box = hit.transform.GetComponent<Box>();

                 if(box != null)
                 {
                     Debug.Log("Cosas");
                 }
             }
         }

     }*/

            //Raycast Avanzado

            IDamageable damageable = hit.transform.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(5);
            }
        }
    }

}
