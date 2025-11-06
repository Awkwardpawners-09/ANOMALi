using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class SceneManagerScript : MonoBehaviour
{
    // This method should be public so it appears in the OnClick() options
    public void OnStartButtonPressed()
    {
        // Load the "BaseScene"
        SceneManager.LoadScene("BaseScene");
    }

    // This method should also be public
    public void OnLeaveButtonPressed()
    {
        // Exit the game
        Application.Quit();

        // In the editor, stop play mode to simulate quitting the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
