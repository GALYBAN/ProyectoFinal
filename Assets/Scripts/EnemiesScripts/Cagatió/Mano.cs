using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Mano : MonoBehaviour
{
    [Header("Movement Settings")]
    public float horizontalSpeed = 12f;
    public float fallSpeed = 20f;
    public float riseSpeed = 15f;
    public float fallActivationDistance = 1f;
    public float groundHeight = 0.5f;
    public float timeBetweenAttacks = 2f;
    public float minDistanceToPlayer = 0.5f;

    [Header("Collision Settings")]
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public GameObject impactEffect;

    public GameObject player;
    private bool isActive = false;
    private bool isFalling = false;
    private bool isRising = false;
    private Rigidbody rb;
    private Collider col;
    private Vector3 targetPosition;
    private float lastAttackTime;
    private Vector3 lastPlayerPosition;
    private float initialHeight;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ConfigurePhysics();
    }

    void Start()
    {
        if (player == null)
        {
            player = GameObject.Find("CleoArmature");
            if (player == null)
            {
                Debug.LogError("Player not found!");
                return;
            }
        }

        initialHeight = transform.position.y;
        lastPlayerPosition = player.transform.position;
        lastAttackTime = Time.time;
    }

    void ConfigurePhysics()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        if (!isActive || player == null) return;

        if (Vector3.Distance(lastPlayerPosition, player.transform.position) > minDistanceToPlayer)
        {
            lastPlayerPosition = player.transform.position;
        }

        if (!isFalling && !isRising)
        {
            MoveHorizontally();
        }
        else if (isFalling)
        {
            FallDown();
        }
        else if (isRising)
        {
            RiseUp();
        }
    }

    void MoveHorizontally()
    {
        Vector3 targetPos = new Vector3(
            lastPlayerPosition.x,
            initialHeight,
            lastPlayerPosition.z
        );

        Vector3 direction = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPos);

        transform.position += direction * horizontalSpeed * Time.deltaTime;

        if (distance < 0.1f)
        {
            transform.position = targetPos;
        }

        float horizontalDistance = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(lastPlayerPosition.x, lastPlayerPosition.z)
        );

        if (horizontalDistance < fallActivationDistance && Time.time - lastAttackTime >= timeBetweenAttacks)
        {
            targetPosition = new Vector3(
                transform.position.x,
                groundHeight,
                transform.position.z
            );
            isFalling = true;
            lastAttackTime = Time.time;
        }
    }

    void FallDown()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * fallSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isFalling = false;
            isRising = true;
            targetPosition = new Vector3(
                transform.position.x,
                initialHeight,
                transform.position.z
            );
        }
    }

    void RiseUp()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * riseSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isRising = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage();
            }
        }
    }

    public void Activate()
    {
        isActive = true;
    }
}