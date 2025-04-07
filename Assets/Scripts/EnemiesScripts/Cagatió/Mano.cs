using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Mano : MonoBehaviour
{
    [Header("Movement Settings")]
    public float horizontalSpeed = 8f;
    public float fallSpeed = 15f;
    public float startingHeight = 15f;
    public float fallActivationDistance = 3f;

    [Header("Collision Settings")]
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public float destroyDelay = 0.1f;
    public GameObject impactEffect;

    public GameObject player;
    private bool isActive = false;
    private bool isFalling = false;
    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ConfigurePhysics();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Debug.LogError("Player not found!");

        transform.position = new Vector3(
            transform.position.x, 
            startingHeight, 
            transform.position.z
        );
    }

    void Update()
    {
        if (!isActive) return;

        if (!isFalling)
        {
            MoveHorizontally();
        }
        else
        {
            FallDown();
        }
    }

    void ConfigurePhysics()
    {
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        col.isTrigger = false; // Importante para colisiones sólidas
    }

    void MoveHorizontally()
    {
        if (player == null) return;

        Vector3 targetPos = new Vector3(
            player.transform.position.x,
            startingHeight,
            player.transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            horizontalSpeed * Time.deltaTime
        );

        // Check distance to start falling
        float horizontalDistance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(player.transform.position.x, player.transform.position.z)
        );

        if (horizontalDistance < fallActivationDistance)
        {
            isFalling = true;
        }
    }

    void FallDown()
    {
        rb.velocity = Vector3.down * fallSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verificar por layer primero (más eficiente)
        int collisionLayer = collision.gameObject.layer;
        
        if (groundLayer == (groundLayer | (1 << collisionLayer)))
        {
            DestroyHand();
        }
        else if (playerLayer == (playerLayer | (1 << collisionLayer)))
        {
            DealDamage(collision.gameObject);
            DestroyHand();
        }
    }

    void DealDamage(GameObject playerObj)
    {
        PlayerStats stats = playerObj.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage();
            Debug.Log("Player damaged by hand!");
        }
    }

    void DestroyHand()
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject, destroyDelay);
    }

    public void Activate()
    {
        isActive = true;
    }
}