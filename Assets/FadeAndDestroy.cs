using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For RawImage component


public class FadeAndDestroyRawImage : MonoBehaviour
{
    public RawImage rawImageToAffect; // The RawImage to be affected (set in inspector)
    public float fadeDuration = 4f;  // Duration for the fade to complete
    public float waitBeforeDestroy = 5f;  // Time to wait after fade before destroying the GameObject

    void Start()
    {
        if (rawImageToAffect != null)
        {
            // Start the fade and destroy sequence
            StartCoroutine(FadeOutAndDestroy());
        }
        else
        {
            Debug.LogError("RawImage to affect is not assigned!");
        }
    }

    // Coroutine to handle the fading and destruction
    private IEnumerator FadeOutAndDestroy()
    {
        Color startColor = rawImageToAffect.color; // Get the initial color
        float timeElapsed = 0f;

        // Fade out the RawImage (transparency from 1 to 0)
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);

            // Set the new color with the updated alpha value
            rawImageToAffect.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Ensure the alpha is exactly 0 after the fade duration
        rawImageToAffect.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Wait for the additional time before destroying the object
        yield return new WaitForSeconds(waitBeforeDestroy);

        // Destroy the GameObject
        Destroy(rawImageToAffect.gameObject);
    }
}
