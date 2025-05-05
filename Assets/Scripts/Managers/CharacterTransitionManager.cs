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
            
            // Set initial camera target based on which character is active
            if (cleoCharacter.activeSelf)
            {
                virtualCamera.Follow = cleoCharacter.transform;
                virtualCamera.LookAt = cleoCharacter.transform;
            }
            else if (poweredCharacter.activeSelf)
            {
                virtualCamera.Follow = poweredCharacter.transform;
                virtualCamera.LookAt = poweredCharacter.transform;
            }
        }
    }

    private void Update()
    {
        if (isInCinematic)
        {
            HandleCinematic();
        }
    }

    public void StartCharacterSwitch()
    {
        if (!isInCinematic && virtualCamera != null)
        {
            isInCinematic = true;
            cinematicTimer = 0f;
            
            // Disable Cleo's components during cinematic
            DisableCleoComponents();
            
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
                StartCoroutine(PlayTransitionSequence());
            }
        }
    }

    private IEnumerator PlayTransitionSequence()
    {
        // Wait for camera to reach cinematic position
        yield return new WaitForSeconds(cinematicDuration * 0.5f);
        
        // Play UI transition
        if (transitionUI != null)
        {
            yield return StartCoroutine(transitionUI.PlayTransitionSequence());
        }
        
        // Wait for camera to return to original position
        yield return new WaitForSeconds(cinematicDuration * 0.5f);
        
        // End cinematic
        EndCinematic();
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
        var playerInputs = poweredCharacter.GetComponent<PlayerInputs>();
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

    private void HandleCinematic()
    {
        if (virtualCamera == null) return;
        
        cinematicTimer += Time.deltaTime;
        float progress = cinematicTimer / cinematicDuration;
        
        if (progress <= 0.5f)
        {
            // First half: Move to cinematic position
            float moveProgress = progress * 2f;
            if (transposer != null)
            {
                transposer.m_FollowOffset = Vector3.Lerp(originalFollowOffset, Vector3.zero, moveProgress);
            }
            virtualCamera.transform.position = Vector3.Lerp(virtualCamera.transform.position, cinematicCameraPosition.position, moveProgress);
            virtualCamera.transform.rotation = Quaternion.Lerp(originalRotation, cinematicCameraPosition.rotation, moveProgress);
        }
        else
        {
            // Second half: Return to follow position
            float returnProgress = (progress - 0.5f) * 2f;
            if (transposer != null)
            {
                transposer.m_FollowOffset = Vector3.Lerp(Vector3.zero, originalFollowOffset, returnProgress);
            }
            virtualCamera.transform.rotation = Quaternion.Lerp(cinematicCameraPosition.rotation, originalRotation, returnProgress);
        }
    }

    private void EndCinematic()
    {
        isInCinematic = false;
        
        // Switch characters
        cleoCharacter.SetActive(false);
        poweredCharacter.SetActive(true);
        
        // Position powered character at Cleo's last position
        poweredCharacter.transform.position = cleoCharacter.transform.position;
        poweredCharacter.transform.rotation = cleoCharacter.transform.rotation;
        
        // Enable all components on powered character
        EnablePoweredCharacterComponents();
        
        // Set camera to follow powered character
        if (virtualCamera != null)
        {
            virtualCamera.Follow = poweredCharacter.transform;
            virtualCamera.LookAt = poweredCharacter.transform;
            if (transposer != null)
            {
                transposer.m_FollowOffset = originalFollowOffset;
            }
            virtualCamera.transform.rotation = originalRotation;
        }
    }

    private void EnableCleoComponents()
    {
        // Enable all components on Cleo
        var cleoController = cleoCharacter.GetComponent<CleoController>();
        var playerInputs = cleoCharacter.GetComponent<PlayerInputs>();
        var animator = cleoCharacter.GetComponent<Animator>();
        
        if (cleoController != null) cleoController.enabled = true;
        if (playerInputs != null) playerInputs.enabled = true;
        if (animator != null) animator.enabled = true;
    }
} 