using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AnomalyGameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject floorTextObject;
    private TextMeshPro floorText;

    [SerializeField] private TextMeshProUGUI debugUIText;

    [Header("Button Colliders")]
    [SerializeField] private GameObject upButton;
    [SerializeField] private GameObject downButton;
    [SerializeField] private GameObject cameraCollider;

    [Header("Floor Setup")]
    [Tooltip("Original floor")]
    [SerializeField] private GameObject originalFloor;

    [Tooltip("Anomaly floors")]
    [SerializeField] private List<GameObject> anomalyFloors = new List<GameObject>();

    [Tooltip("The exit floor - appears when reaching floor 0")]
    [SerializeField] private GameObject exitFloor;

    [Header("Elevator Animation")]
    [SerializeField] private GameObject elevatorAnimObject;
    [SerializeField] private AudioClip elevatorCloseSound;
    [SerializeField] private AudioClip elevatorOpenSound;

    private Animator elevatorAnimator;
    private AudioSource audioSource;

    [Header("Settings")]
    [SerializeField] private int startingFloor = 10;
    [SerializeField] private float buttonCooldown = 0.5f;

    [Tooltip("Initial chance (0-100) for the original floor to appear.")]
    [Range(0, 100)]
    [SerializeField] private float originalFloorChance = 30f;

    [Tooltip("Divide original floor chance by this number each time it appears")]
    [SerializeField] private float originalFloorChanceDivider = 2f;

    private int currentFloor;
    private GameObject activeAnomaly;
    private bool hasAnomaly;
    private bool canPressButton = true;
    private bool gameStarted = false;

    // Track current modified chance for original floor
    private float currentOriginalFloorChance;

    // Track remaining anomaly floors that haven't been encountered
    private List<GameObject> remainingAnomalyFloors = new List<GameObject>();

    // Track which button the player is currently touching
    private bool isTouchingUpButton = false;
    private bool isTouchingDownButton = false;

    void Start()
    {
        // Get TextMeshPro component from the 3D text object
        if (floorTextObject != null)
        {
            floorText = floorTextObject.GetComponent<TextMeshPro>();
        }

        // Get Animator component
        if (elevatorAnimObject != null)
        {
            elevatorAnimator = elevatorAnimObject.GetComponent<Animator>();
        }

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        currentFloor = startingFloor;
        currentOriginalFloorChance = originalFloorChance;
        UpdateFloorText();

        // Disable debug UI at start
        if (debugUIText != null)
        {
            debugUIText.gameObject.SetActive(false);
        }

        // Initialize remaining anomaly floors list
        ResetAnomalyFloorPool();

        // Start with original floor only
        ShowOriginalFloor();
    }

    void Update()
    {
        CheckCollisions();
        HandleButtonInput();
    }

    void CheckCollisions()
    {
        if (cameraCollider == null) return;

        bool wasColliding = isTouchingUpButton || isTouchingDownButton;
        isTouchingUpButton = false;
        isTouchingDownButton = false;

        Collider camCollider = cameraCollider.GetComponent<Collider>();
        if (camCollider == null) return;

        // Check collision with up button
        if (upButton != null)
        {
            Collider upCollider = upButton.GetComponent<Collider>();
            if (upCollider != null && camCollider.bounds.Intersects(upCollider.bounds))
            {
                isTouchingUpButton = true;
            }
        }

        // Check collision with down button
        if (downButton != null)
        {
            Collider downCollider = downButton.GetComponent<Collider>();
            if (downCollider != null && camCollider.bounds.Intersects(downCollider.bounds))
            {
                isTouchingDownButton = true;
            }
        }

        // Update UI when collision state changes
        bool isColliding = isTouchingUpButton || isTouchingDownButton;
        if (wasColliding != isColliding)
        {
            UpdateDebugUI();
        }
    }

    void UpdateDebugUI()
    {
        if (debugUIText != null)
        {
            if (isTouchingUpButton)
            {
                debugUIText.text = "Press E - UP (Anomaly Detected)";
                debugUIText.gameObject.SetActive(true);
            }
            else if (isTouchingDownButton)
            {
                debugUIText.text = "Press E - DOWN (No Anomaly)";
                debugUIText.gameObject.SetActive(true);
            }
            else
            {
                debugUIText.gameObject.SetActive(false);
            }
        }
    }

    void ShowOriginalFloor()
    {
        // Disable all floors first
        if (originalFloor != null)
        {
            originalFloor.SetActive(false);
        }

        foreach (GameObject anomalyFloor in anomalyFloors)
        {
            if (anomalyFloor != null)
            {
                anomalyFloor.SetActive(false);
            }
        }

        // Show only original floor
        if (originalFloor != null)
        {
            originalFloor.SetActive(true);
        }

        hasAnomaly = false;
        activeAnomaly = null;
        Debug.Log("Original floor active - No anomaly");
    }

    void ResetAnomalyFloorPool()
    {
        // Reset the pool to include all anomaly floors
        remainingAnomalyFloors.Clear();
        foreach (GameObject floor in anomalyFloors)
        {
            if (floor != null)
            {
                remainingAnomalyFloors.Add(floor);
            }
        }

        // Reset original floor chance back to starting value
        currentOriginalFloorChance = originalFloorChance;

        Debug.Log($"Anomaly floor pool reset. {remainingAnomalyFloors.Count} anomaly floors available. Original floor chance: {currentOriginalFloorChance}%");
    }

    void SetupRandomAnomaly()
    {
        // Disable original floor
        if (originalFloor != null)
        {
            originalFloor.SetActive(false);
        }

        // Disable all anomaly floors
        foreach (GameObject anomalyFloor in anomalyFloors)
        {
            if (anomalyFloor != null)
            {
                anomalyFloor.SetActive(false);
            }
        }

        // If no anomaly floors remain, must show original
        if (remainingAnomalyFloors.Count == 0)
        {
            Debug.Log("No anomaly floors remaining - showing original floor");
            hasAnomaly = false;
            if (originalFloor != null)
            {
                originalFloor.SetActive(true);
            }
            activeAnomaly = null;
            return;
        }

        // Use current modified chance for original floor
        float roll = Random.Range(0f, 100f);
        bool showOriginalFloor = roll < currentOriginalFloorChance;

        Debug.Log($"Roll: {roll:F2} | Original floor chance: {currentOriginalFloorChance:F2}% | Show original: {showOriginalFloor}");

        if (showOriginalFloor)
        {
            // Show original floor
            hasAnomaly = false;
            if (originalFloor != null)
            {
                originalFloor.SetActive(true);
                Debug.Log($"Original floor active - No anomaly (appeared at {currentOriginalFloorChance:F2}% chance)");
            }
            activeAnomaly = null;

            // Reduce chance for next time (halve it)
            currentOriginalFloorChance /= originalFloorChanceDivider;
            Debug.Log($"Original floor chance reduced to: {currentOriginalFloorChance:F2}%");
        }
        else
        {
            // Show anomaly floor - pick from remaining pool
            hasAnomaly = true;

            int randomIndex = Random.Range(0, remainingAnomalyFloors.Count);
            activeAnomaly = remainingAnomalyFloors[randomIndex];
            activeAnomaly.SetActive(true);

            // Reset original floor chance when anomaly appears
            currentOriginalFloorChance = originalFloorChance;
            Debug.Log($"Anomaly floor active: {activeAnomaly.name} ({remainingAnomalyFloors.Count} remaining in pool) - Original floor chance reset to {currentOriginalFloorChance}%");
        }
    }

    void HandleButtonInput()
    {
        if (!canPressButton) return;

        // Check if player presses E while touching a button
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTouchingUpButton)
            {
                OnUpButtonPressed();
            }
            else if (isTouchingDownButton)
            {
                OnDownButtonPressed();
            }
        }
    }

    void OnUpButtonPressed()
    {
        Debug.Log("UP button pressed - Player pressed UP");

        // UP should only be pressed when anomaly floor is active
        // Check if an anomaly floor is currently enabled (not the original floor)
        bool isCorrect = false;

        if (originalFloor != null && !originalFloor.activeSelf)
        {
            // Correct! An anomaly floor is active (original is disabled)
            Debug.Log("CORRECT! Anomaly floor detected!");
            isCorrect = true;
        }
        else
        {
            // Wrong! Normal floor is active
            Debug.Log("WRONG! This is the normal floor!");
            isCorrect = false;
        }

        StartCoroutine(ElevatorSequence(isCorrect));
        gameStarted = true;
    }

    void OnDownButtonPressed()
    {
        Debug.Log("DOWN button pressed - Player pressed DOWN");

        // DOWN should only be pressed when normal floor is active
        // Check if original floor is currently enabled
        bool isCorrect = false;

        if (originalFloor != null && originalFloor.activeSelf)
        {
            // Correct! Normal floor is active
            Debug.Log("CORRECT! Normal floor detected!");
            isCorrect = true;
        }
        else
        {
            // Wrong! An anomaly floor is active
            Debug.Log("WRONG! There is an anomaly!");
            isCorrect = false;
        }

        StartCoroutine(ElevatorSequence(isCorrect));
        gameStarted = true;
    }

    IEnumerator ElevatorSequence(bool correctAnswer)
    {
        canPressButton = false;

        // Hide debug UI during elevator sequence
        if (debugUIText != null)
        {
            debugUIText.gameObject.SetActive(false);
        }

        // Play elevator close animation
        if (elevatorAnimator != null)
        {
            elevatorAnimator.Play("elevatorclose");
        }

        // Play close sound
        if (elevatorCloseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(elevatorCloseSound);
        }

        // Wait 2 seconds while elevator closes
        yield return new WaitForSeconds(2f);

        // While elevator is closed, handle floor logic and reload
        if (correctAnswer)
        {
            HandleCorrectChoice();
        }
        else
        {
            HandleIncorrectChoice();
        }

        // Wait another 2 seconds
        yield return new WaitForSeconds(2f);

        // Play elevator open animation
        if (elevatorAnimator != null)
        {
            elevatorAnimator.Play("elevatoropen");
        }

        // Play open sound
        if (elevatorOpenSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(elevatorOpenSound);
        }

        // Small delay before allowing button press again
        yield return new WaitForSeconds(buttonCooldown);
        canPressButton = true;
    }

    void HandleCorrectChoice()
    {
        // If an anomaly floor was correctly identified, remove it from the pool
        if (hasAnomaly && activeAnomaly != null && remainingAnomalyFloors.Contains(activeAnomaly))
        {
            remainingAnomalyFloors.Remove(activeAnomaly);
            Debug.Log($"Anomaly floor {activeAnomaly.name} removed from pool. {remainingAnomalyFloors.Count} remaining.");
        }

        // Player was correct - decrease floor by 1
        currentFloor--;

        if (currentFloor <= 0)
        {
            Debug.Log("YOU WIN! Reached floor 0!");
            if (floorText != null)
            {
                floorText.text = "0";
            }
            ShowExitFloor();
            return;
        }

        UpdateFloorText();
        SetupRandomAnomaly();
    }

    void ShowExitFloor()
    {
        // Disable all other floors
        if (originalFloor != null)
        {
            originalFloor.SetActive(false);
        }

        foreach (GameObject anomalyFloor in anomalyFloors)
        {
            if (anomalyFloor != null)
            {
                anomalyFloor.SetActive(false);
            }
        }

        // Show only the exit floor
        if (exitFloor != null)
        {
            exitFloor.SetActive(true);
            Debug.Log("Exit floor activated - You can now escape!");
        }

        // Disable button functionality
        canPressButton = false;
    }

    void HandleIncorrectChoice()
    {
        // Player was wrong - reset to floor 10 and reset anomaly pool
        currentFloor = startingFloor;
        ResetAnomalyFloorPool();
        UpdateFloorText();
        SetupRandomAnomaly();
    }

    void UpdateFloorText()
    {
        if (floorText != null)
        {
            floorText.text = currentFloor.ToString();
        }
    }

    // Public methods called by ButtonTriggerDetector
    public void OnEnterUpButton()
    {
        isTouchingUpButton = true;
        UpdateDebugUI();
        Debug.Log("Touching UP button - Press E to select");
    }

    public void OnExitUpButton()
    {
        isTouchingUpButton = false;
        UpdateDebugUI();
    }

    public void OnEnterDownButton()
    {
        isTouchingDownButton = true;
        UpdateDebugUI();
        Debug.Log("Touching DOWN button - Press E to select");
    }

    public void OnExitDownButton()
    {
        isTouchingDownButton = false;
        UpdateDebugUI();
    }

    // Public methods for external access
    public int GetCurrentFloor() => currentFloor;
    public bool HasActiveAnomaly() => hasAnomaly;
    public GameObject GetActiveAnomaly() => activeAnomaly;
}