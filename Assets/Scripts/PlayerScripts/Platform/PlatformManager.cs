using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private GameObject platformPrefab; // Prefab de la plataforma
    [SerializeField] private LayerMask groundLayer; // Capa del suelo
    [SerializeField] private LayerMask collisionLayers; // Capas que bloquearán la creación
    [SerializeField] private Vector3 spawnOffset = new Vector3(2, 0, 0); // Offset inicial de la plataforma

    private GameObject[] platformPool; // Array de plataformas
    private GameObject currentPlatform; // Plataforma actualmente activa
    private bool isPlacingPlatform = false; // Indica si estás colocando la plataforma

    private GlobalPlayerController playerController; // Referencia al controlador del jugador
    private PlayerInputs playerInputs;
    private GroundSensor groundSensor;

    void Awake()
    {
        groundSensor = FindObjectOfType<GroundSensor>();
        playerController = FindObjectOfType<GlobalPlayerController>();
        playerInputs = FindObjectOfType<PlayerInputs>();

        // Crear el pool de plataformas con solo 2 plataformas
        platformPool = new GameObject[2];
        platformPool[0] = Instantiate(platformPrefab);
        platformPool[1] = Instantiate(platformPrefab);

        // Desactivar las plataformas al principio
        platformPool[0].SetActive(false);
        platformPool[1].SetActive(false);
    }

    void Update()
    {
        // Si se presiona la tecla para crear una plataforma y no hay ninguna activa
        if (playerInputs.TogglePlatformInput)
        {
            if (CanCreatePlatform())
            {
                TogglePlatform();
            }
            else if (isPlacingPlatform) // Si ya está creando una plataforma, se puede cancelar
            {
                CancelPlatformCreation();
            }
        }

        // Si estamos colocando la plataforma, moverla y colocarla
        if (isPlacingPlatform && currentPlatform != null)
        {
            MovePlatform();

            // Confirmar colocación de la plataforma
            if (playerInputs.ConfirmPlatformPlacement)
            {
                PlacePlatform();
            }
        }
    }

    bool CanCreatePlatform()
    {
        bool isGrounded = groundSensor.IsGrounded();
        bool onGroundLayer = IsOnLayer(groundLayer);

        // Solo permite crear la plataforma si el jugador está tocando el suelo y no hay plataforma activa
        return isGrounded && onGroundLayer && !isPlacingPlatform;
    }

    bool IsOnLayer(LayerMask layerMask)
    {
        Vector3 playerPosition = groundSensor.sensorPosition.position;
        return Physics.CheckSphere(playerPosition, groundSensor.sensorRadius, layerMask);
    }

    void TogglePlatform()
    {
        // Si ya hay una plataforma activa, la desactivamos
        if (currentPlatform != null)
        {
            currentPlatform.SetActive(false); // Desactivamos la plataforma anterior
        }

        // Obtener la siguiente plataforma en el pool
        currentPlatform = GetNextPlatform();

        // Convertir la posición del jugador a la posición local y aplicar el offset
        Vector3 spawnPosition = playerController.transform.TransformPoint(spawnOffset);

        // Colocar la plataforma en la posición local calculada
        currentPlatform.transform.position = spawnPosition;
        currentPlatform.SetActive(true); // Activar la nueva plataforma

        isPlacingPlatform = true;
        playerController.enabled = false; // Desactivar el control del jugador mientras coloca la plataforma
    }

    GameObject GetNextPlatform()
    {
        // Verifica cuál plataforma del pool está inactiva y la devuelve
        if (!platformPool[0].activeInHierarchy)
        {
            return platformPool[0];
        }
        else
        {
            return platformPool[1];
        }
    }

    void MovePlatform()
    {
        Vector3 newPosition = currentPlatform.transform.position;

        // Mueve la plataforma según los inputs
        newPosition.x += playerInputs.PlatformMovementInput.x * 5f * Time.deltaTime;
        newPosition.y += playerInputs.PlatformMovementInput.y * 5f * Time.deltaTime;

        Collider[] hits = Physics.OverlapBox(newPosition, currentPlatform.transform.localScale / 2, Quaternion.identity, collisionLayers);
        if (hits.Length == 0)
        {
            currentPlatform.transform.position = newPosition;
        }
    }

    void PlacePlatform()
    {
        // Confirmar la colocación y vuelve a permitir la creación de plataformas
        isPlacingPlatform = false;
        playerController.enabled = true; // Reactivar el control del jugador
    }

    // Método para cancelar la creación de la plataforma
    void CancelPlatformCreation()
    {
        if (currentPlatform != null)
        {
            currentPlatform.SetActive(false); // Desactivamos la plataforma en proceso de creación
            isPlacingPlatform = false; // Dejamos de colocar la plataforma
            playerController.enabled = true; // Reactivamos el control del jugador
        }
    }
}
