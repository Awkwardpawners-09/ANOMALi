using UnityEngine;
using System.Collections.Generic;

public class AnomalyTrigger : MonoBehaviour
{
    [Header("Collision Detection")]
    [Tooltip("The player GameObject to detect collision with")]
    [SerializeField] private GameObject playerObject;

    [Tooltip("The trigger GameObject that the player must collide with")]
    [SerializeField] private GameObject triggerObject;

    [Header("Objects to Enable")]
    [Tooltip("List of GameObjects to enable when collision happens")]
    [SerializeField] private List<GameObject> objectsToEnable = new List<GameObject>();

    [Header("Objects to Disable")]
    [Tooltip("List of GameObjects to disable when collision happens")]
    [SerializeField] private List<GameObject> objectsToDisable = new List<GameObject>();

    [Header("Sounds to Play")]
    [Tooltip("List of AudioClips to play when collision happens")]
    [SerializeField] private List<AudioClip> soundsToPlay = new List<AudioClip>();

    [Header("Settings")]
    [SerializeField] private bool triggerOnce = false;
    [SerializeField] private bool useTriggerCollision = true;

    private AudioSource audioSource;
    private bool hasTriggered = false;

    void Start()
    {
        // Create AudioSource for playing sounds
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (!useTriggerCollision)
        {
            CheckBoundsCollision();
        }
    }

    void CheckBoundsCollision()
    {
        if (hasTriggered && triggerOnce) return;
        if (playerObject == null || triggerObject == null) return;

        Collider playerCollider = playerObject.GetComponent<Collider>();
        Collider triggerCollider = triggerObject.GetComponent<Collider>();

        if (playerCollider != null && triggerCollider != null)
        {
            if (playerCollider.bounds.Intersects(triggerCollider.bounds))
            {
                ExecuteTriggerActions();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTriggerCollision) return;
        if (hasTriggered && triggerOnce) return;

        // Check if the colliding object is the player
        if (playerObject != null && other.gameObject == playerObject)
        {
            ExecuteTriggerActions();
        }
    }

    void ExecuteTriggerActions()
    {
        Debug.Log($"Collision trigger activated!");

        // Enable objects
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Enabled: {obj.name}");
            }
        }

        // Disable objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Debug.Log($"Disabled: {obj.name}");
            }
        }

        // Play sounds
        foreach (AudioClip clip in soundsToPlay)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
                Debug.Log($"Playing sound: {clip.name}");
            }
        }

        hasTriggered = true;
    }

    // Public method to reset the trigger
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}