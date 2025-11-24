using UnityEngine;
using UnityEngine.InputSystem;

public class RayCastRepaso : MonoBehaviour
{
// Que cuando hagas click en un objeto el raycast detecte en que has hecho click y pase algo , importante tenr input de ataque y de posición de raton   

InputAction _clickAcion;

InputAction _positionAction;

Vector2 _mousePoition;


void Awake()
{
    _clickAcion = InputSystem.actions["Attack"];
    _positionAction = InputSystem.actions ["MousePosition"];
}

void Update()
{
    _mousePoition = _positionAction.ReadValue<Vector2>();

    if(_clickAcion.WasPerformedThisFrame())
    {
        ShootRaycast();
    }
}

void ShootRaycast()
{
    Ray ray = Camera.main.ScreenPointToRay(_mousePoition);
    RaycastHit hit;
    if(Physics.Raycast(ray, out hit, Mathf.Infinity))
    {
        if(hit.transform.gameObject.layer == 3) //Crear layer para cada objeto para diferenciarlos (3ifs diferentes)
        {
            Debug.Log(hit.transform.name); // Para diferenciar el objeto que pulsas
        }
        if(hit.transform.gameObject.tag == "effne") // Crear tags para cada objeto para diferenciarlos (3ifs diferentes)
        {

        }
        if(hit.transform.name == "efjfqj") // Usar nombres de los objetos para diferenciarlos (3ifs diferentes)º
        {

        }
    }
}
 
}




