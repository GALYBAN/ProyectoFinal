using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Wall Detection")]
    [SerializeField] private Transform sensorPosition; // Punto desde donde salen los rayos
    [SerializeField] private float raySideSize = 0.5f; // Longitud del rayo
    [SerializeField] private float slideSpeed = 5f; // Velocidad de deslizamiento
    [SerializeField] private LayerMask collisionMask = -1; // Capas con las que colisionar

    private CharacterController characterController;
    private bool isTouchingWall;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Si no hay un punto de sensor asignado, crear uno
        if (sensorPosition == null)
        {
            GameObject sensorObj = new GameObject("WallSensor");
            sensorObj.transform.parent = transform;
            sensorObj.transform.localPosition = new Vector3(0, 0.5f, 0); // Ajusta esta altura según tu personaje
            sensorPosition = sensorObj.transform;
        }
    }

    private void Update()
    {
        CheckWallCollision();
        CheckCorner();
    }

    private void CheckWallCollision()
    {
        if (characterController == null) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        if (horizontalInput == 0)
        {
            isTouchingWall = false;
            return;
        }

        // Lanzar raycast en la dirección del movimiento
        Vector3 direction = transform.right * horizontalInput;
        
        Debug.DrawRay(sensorPosition.position, direction * raySideSize, Color.red);

        if (Physics.Raycast(sensorPosition.position, direction, out RaycastHit hit, raySideSize, collisionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.isTrigger) return;

            isTouchingWall = true;
            SlideAlongWall(hit.normal);
        }
        else
        {
            isTouchingWall = false;
        }
    }

    private void CheckCorner()
    {
        RaycastHit hit;
        // Comprobar adelante y atrás
        if (Physics.Raycast(sensorPosition.position, transform.forward, out hit, raySideSize, collisionMask, QueryTriggerInteraction.Ignore) || 
            Physics.Raycast(sensorPosition.position, -transform.forward, out hit, raySideSize, collisionMask, QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.isTrigger)
            {
                SlideAlongWall(hit.normal);
            }
        }

        // Dibujar rayos de debug
        Debug.DrawRay(sensorPosition.position, transform.forward * raySideSize, Color.blue);
        Debug.DrawRay(sensorPosition.position, -transform.forward * raySideSize, Color.blue);
    }

    private void SlideAlongWall(Vector3 wallNormal)
    {
        // Calcular la dirección de deslizamiento y aplicar el movimiento
        Vector3 slideDirection = wallNormal;
        characterController.Move((slideDirection * slideSpeed + Physics.gravity) * Time.deltaTime);
    }

    public bool IsTouchingWall()
    {
        return isTouchingWall;
    }

    public bool IsWallAhead(float direction)
    {
        Vector3 rayDirection = transform.right * direction;
        return Physics.Raycast(sensorPosition.position, rayDirection, raySideSize, collisionMask, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        if (sensorPosition != null)
        {
            // Visualizar el punto del sensor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(sensorPosition.position, 0.1f);
            
            // Visualizar el rango de detección
            Gizmos.color = Color.red;
            Gizmos.DrawLine(sensorPosition.position, sensorPosition.position + transform.right * raySideSize);
            Gizmos.DrawLine(sensorPosition.position, sensorPosition.position - transform.right * raySideSize);
        }
    }
}
