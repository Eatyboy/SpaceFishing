using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader instance;

    [SerializeField] private Image image;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        image.color = new(0, 0, 0, 0);
        image.raycastTarget = false;
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            image.color = new(0, 0, 0, 1.0f - t);
            yield return null;
        }

        image.raycastTarget = false;
    }

    public IEnumerator FadeOut()
    {
        image.raycastTarget = true;

        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            image.color = new(0, 0, 0, t);
            yield return null;
        }
    }
}
