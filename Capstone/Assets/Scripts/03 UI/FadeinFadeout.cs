using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeinFadeout : MonoBehaviour
{
    private CanvasGroup fadeCanvas;
    [SerializeField]
    private float fadeDuration = 1.5f;

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(0.0f, 230.0f, elapsedTime / fadeDuration) / 255.0f;
            yield return null;
        }
        fadeCanvas.alpha = 230.0f / 255.0f;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0.0f;
        fadeCanvas.alpha = 230.0f / 255.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(230.0f, 0.0f, elapsedTime / fadeDuration) / 255.0f;
            yield return null;
        }
        fadeCanvas.alpha = 0.0f;
    }

    IEnumerator FadeBattleUI()
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeOut());
    }

    public void ShowBattleUI()
    {
        // fadeCanvas = GetComponent<CanvasGroup>();
        StartCoroutine(FadeBattleUI());
    }

    private void Start()
    {
        fadeCanvas = GetComponent<CanvasGroup>();
        fadeCanvas.alpha = 0.0f;
        // StartCoroutine(FadeBattleUI());
    }
}
