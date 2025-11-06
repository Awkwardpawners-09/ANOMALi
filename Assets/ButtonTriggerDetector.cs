using UnityEngine;

public class ButtonTriggerDetector : MonoBehaviour
{
    [SerializeField] private AnomalyGameManager gameManager;
    [SerializeField] private bool isUpButton = true; // Toggle this in inspector

    void OnTriggerEnter(Collider other)
    {
        // Check if the camera collider entered
        if (other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            if (gameManager != null)
            {
                if (isUpButton)
                {
                    gameManager.OnEnterUpButton();
                }
                else
                {
                    gameManager.OnEnterDownButton();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the camera collider exited
        if (other.CompareTag("MainCamera") || other.GetComponent<Camera>() != null)
        {
            if (gameManager != null)
            {
                if (isUpButton)
                {
                    gameManager.OnExitUpButton();
                }
                else
                {
                    gameManager.OnExitDownButton();
                }
            }
        }
    }
}