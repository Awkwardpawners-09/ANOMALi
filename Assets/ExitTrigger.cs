using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    [Header("Collision Detection")]
    [Tooltip("The player GameObject to detect collision with")]
    [SerializeField] private GameObject playerObject;

    [Tooltip("The trigger GameObject that the player must collide with")]
    [SerializeField] private GameObject triggerObject;

    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load")]
    [SerializeField] private string sceneToLoad = "Main Menu";

    [Header("Settings")]
    [SerializeField] private bool useTriggerCollision = true;
    [SerializeField] private bool changeSceneOnce = true;

    private bool hasChangedScene = false;

    void Update()
    {
        if (!useTriggerCollision)
        {
            CheckBoundsCollision();
        }
    }

    void CheckBoundsCollision()
    {
        if (hasChangedScene && changeSceneOnce) return;
        if (playerObject == null || triggerObject == null) return;

        Collider playerCollider = playerObject.GetComponent<Collider>();
        Collider triggerCollider = triggerObject.GetComponent<Collider>();

        if (playerCollider != null && triggerCollider != null)
        {
            if (playerCollider.bounds.Intersects(triggerCollider.bounds))
            {
                ChangeScene();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTriggerCollision) return;
        if (hasChangedScene && changeSceneOnce) return;

        // Check if the colliding object is the player
        if (playerObject != null && other.gameObject == playerObject)
        {
            ChangeScene();
        }
    }

    void ChangeScene()
    {
        if (hasChangedScene && changeSceneOnce) return;

        Debug.Log($"Changing scene to: {sceneToLoad}");
        hasChangedScene = true;

        // Load the scene
        SceneManager.LoadScene(sceneToLoad);
    }

    // Public method to change scene from other scripts
    public void ChangeScenePublic(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}