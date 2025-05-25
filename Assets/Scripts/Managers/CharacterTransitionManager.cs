using UnityEngine;
using Cinemachine;
using System.Collections;

public class CharacterTransitionManager : MonoBehaviour
{
    [Header("Character References")]
    [SerializeField] private GameObject cleoCharacter;
    [SerializeField] private GameObject poweredCharacter;
    
    [Header("Cinematic Settings")]
    [SerializeField] private float cinematicDuration = 5f;
    [SerializeField] private Transform cinematicCameraPosition;
    
    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float cinematicBlendTime = 1f;
    
    [Header("UI Settings")]
    [SerializeField] private TransitionUI transitionUI;
    
    private CinemachineTransposer transposer;
    private Vector3 originalFollowOffset;
    private Quaternion originalRotation;
    private bool isInCinematic = false;
    private float cinematicTimer = 0f;
    private bool isTransformed = false; // Track transformation state

    private void Start()
    {
        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                originalFollowOffset = transposer.m_FollowOffset;
            }
            originalRotation = virtualCamera.transform.rotation;
            
            // Set initial camera target based on saved state or default
            UpdateCameraTarget();
        }
    }

    private void UpdateCameraTarget()
    {
        if (virtualCamera != null)
        {
            Transform target = isTransformed ? poweredCharacter.transform : cleoCharacter.transform;
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
            Debug.Log($"Camera updated to follow: {target.name}");
        }
    }

    public void StartCharacterSwitch()
    {
        if (!isInCinematic && virtualCamera != null)
        {
            isInCinematic = true;
            cinematicTimer = 0f;
            
            // Store positions before transition
            Vector3 currentPosition = isTransformed ? poweredCharacter.transform.position : cleoCharacter.transform.position;
            
            // Disable current character's components
            if (isTransformed)
            {
                DisablePoweredComponents();
            }
            else
            {
                DisableCleoComponents();
            }
            
            // Store original camera settings
            if (transposer != null)
            {
                originalFollowOffset = transposer.m_FollowOffset;
            }
            originalRotation = virtualCamera.transform.rotation;
            
            // Start cinematic camera movement
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;
            
            // Start UI transition
            if (transitionUI != null)
            {
                StartCoroutine(PlayTransitionSequence(currentPosition));
            }
        }
    }

    private IEnumerator PlayTransitionSequence(Vector3 startPosition)
    {
        // Wait for camera to reach cinematic position
        yield return new WaitForSeconds(cinematicDuration * 0.5f);
        
        // Play UI transition
        if (transitionUI != null)
        {
            yield return StartCoroutine(transitionUI.PlayTransitionSequence());
        }
        
        // Switch characters
        isTransformed = !isTransformed;
        EndCinematic(startPosition);
        
        // Wait for camera to return to original position
        yield return new WaitForSeconds(cinematicDuration * 0.5f);
    }

    private void EndCinematic(Vector3 startPosition)
    {
        isInCinematic = false;
        
        // Switch characters based on transformation state
        cleoCharacter.SetActive(!isTransformed);
        poweredCharacter.SetActive(isTransformed);
        
        // Position the active character at the previous position
        GameObject activeCharacter = isTransformed ? poweredCharacter : cleoCharacter;
        activeCharacter.transform.position = startPosition;
        
        // Enable components for the active character
        if (isTransformed)
        {
            EnablePoweredCharacterComponents();
        }
        else
        {
            EnableCleoComponents();
        }
        
        // Update camera
        UpdateCameraTarget();
        if (transposer != null)
        {
            transposer.m_FollowOffset = originalFollowOffset;
        }
        virtualCamera.transform.rotation = originalRotation;
    }

    private void DisablePoweredComponents()
    {
        if (poweredCharacter != null)
        {
            var components = poweredCharacter.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component != this) // Don't disable the TransitionManager
                {
                    component.enabled = false;
                }
            }
        }
    }

    private void DisableCleoComponents()
    {
        // Disable all relevant components on Cleo
        var cleoController = cleoCharacter.GetComponent<CleoController>();
        var playerInputs = cleoCharacter.GetComponent<PlayerInputs>();
        var animator = cleoCharacter.GetComponent<Animator>();
        
        if (cleoController != null) cleoController.enabled = false;
        if (playerInputs != null) playerInputs.enabled = false;
        if (animator != null) animator.enabled = false;
    }

    private void EnablePoweredCharacterComponents()
    {
        // Enable all components on the powered character
        var globalController = poweredCharacter.GetComponent<GlobalPlayerController>();
        var playerInputs = poweredCharacter.GetComponentInParent<PlayerInputs>();
        var animator = poweredCharacter.GetComponent<Animator>();
        var movementController = poweredCharacter.GetComponent<MovementController>();
        var gravityController = poweredCharacter.GetComponent<PlayerGravity>();
        var groundSensor = poweredCharacter.GetComponent<GroundSensor>();
        var comboController = poweredCharacter.GetComponent<ComboController>();
        var playerStats = poweredCharacter.GetComponent<PlayerStats>();
        
        if (globalController != null) globalController.enabled = true;
        if (playerInputs != null) playerInputs.enabled = true;
        if (animator != null) animator.enabled = true;
        if (movementController != null) movementController.enabled = true;
        if (gravityController != null) gravityController.enabled = true;
        if (groundSensor != null) groundSensor.enabled = true;
        if (comboController != null) comboController.enabled = true;
        if (playerStats != null) playerStats.enabled = true;
    }

    private void EnableCleoComponents()
    {
        // Enable all components on Cleo
        var cleoController = cleoCharacter.GetComponent<CleoController>();
        var playerInputs = cleoCharacter.GetComponentInParent<PlayerInputs>();
        var animator = cleoCharacter.GetComponent<Animator>();
        
        if (cleoController != null) cleoController.enabled = true;
        if (playerInputs != null) playerInputs.enabled = true;
        if (animator != null) animator.enabled = true;
    }

    public GameObject GetUnpoweredCharacter()
    {
        return cleoCharacter;
    }

    public GameObject GetPoweredCharacter()
    {
        return poweredCharacter;
    }

    public void LoadCharacterState(SaveData saveData)
    {
        if (saveData == null)
        {
            Debug.LogWarning("Attempted to load null save data!");
            return;
        }

        Debug.Log($"Loading character state - IsTransformed: {saveData.isTransformed}");
        Debug.Log($"Unpowered Active: {saveData.isUnpoweredActive}, Position: {saveData.unpoweredPosition.ToVector3()}");
        Debug.Log($"Powered Active: {saveData.isPoweredActive}, Position: {saveData.poweredPosition.ToVector3()}");

        // Set characters active state
        if (cleoCharacter != null)
        {
            cleoCharacter.SetActive(saveData.isUnpoweredActive);
            if (saveData.isUnpoweredActive)
            {
                cleoCharacter.transform.position = saveData.unpoweredPosition.ToVector3();
                var stats = cleoCharacter.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.maxHealthSlots = saveData.unpoweredMaxHealth;
                    stats.currentHealthSlots = saveData.unpoweredCurrentHealth;
                    stats.maxManaSlots = saveData.unpoweredMaxMana;
                    stats.currentManaSlots = saveData.unpoweredCurrentMana;
                    Debug.Log($"Loaded Cleo stats - Health: {stats.currentHealthSlots}/{stats.maxHealthSlots}, Mana: {stats.currentManaSlots}/{stats.maxManaSlots}");
                }
            }
        }

        if (poweredCharacter != null)
        {
            poweredCharacter.SetActive(saveData.isPoweredActive);
            if (saveData.isPoweredActive)
            {
                poweredCharacter.transform.position = saveData.poweredPosition.ToVector3();
                var stats = poweredCharacter.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.maxHealthSlots = saveData.poweredMaxHealth;
                    stats.currentHealthSlots = saveData.poweredCurrentHealth;
                    stats.maxManaSlots = saveData.poweredMaxMana;
                    stats.currentManaSlots = saveData.poweredCurrentMana;
                    Debug.Log($"Loaded Powered stats - Health: {stats.currentHealthSlots}/{stats.maxHealthSlots}, Mana: {stats.currentManaSlots}/{stats.maxManaSlots}");
                }
            }
        }

        // Set the transformation state
        isTransformed = saveData.isTransformed;
        Debug.Log($"Set transformation state to: {isTransformed}");

        // Update camera target based on active character
        if (virtualCamera != null)
        {
            Transform target = isTransformed ? poweredCharacter.transform : cleoCharacter.transform;
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
            Debug.Log($"Updated camera to follow: {target.name}");
        }
    }

    public bool IsTransformed()
    {
        return isTransformed;
    }

    public void SetTransformed(bool transformed, Vector3 position)
    {
        Debug.Log($"Setting transformed state to {transformed} at position {position}");
        if (this.isTransformed != transformed)
        {
            this.isTransformed = transformed;
            cleoCharacter.SetActive(!transformed);
            poweredCharacter.SetActive(transformed);
            
            GameObject activeCharacter = transformed ? poweredCharacter : cleoCharacter;
            activeCharacter.transform.position = position;
            
            Debug.Log($"Switched to {(transformed ? "powered" : "unpowered")} character at position {position}");
            UpdateCameraTarget();
        }
    }
} 