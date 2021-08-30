using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader Instance { get; private set; }
    [SerializeField] private Image flashImage;
    [SerializeField] private Image fadeImage;



    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator FadeIn(float time)
    {
        yield return fadeImage.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time)
    {
        yield return fadeImage.DOFade(0f, time).WaitForCompletion();
        Debug.Log("Fade out");
    }

    public IEnumerator StartFlash(float timeBetweenFlash, int numFlashes, Color flashColor)
    {
        // save the InputField.textComponent color
        flashImage.color = flashColor;
        for (int i = 0; i < numFlashes; i++)
        {
            float flashInDuration = timeBetweenFlash / 2;
            for(float t = 0; t <= flashInDuration; t += Time.deltaTime)
            {
                Color colorThisFrame = flashImage.color;
                colorThisFrame.a = Mathf.Lerp(0, 0.7f, t / flashInDuration);
                flashImage.color = colorThisFrame;
                yield return null;
            }

            float flashOutDuration = timeBetweenFlash / 2;
            for (float t = 0; t <= flashOutDuration; t += Time.deltaTime)
            {
                Color colorThisFrame = flashImage.color;
                colorThisFrame.a = Mathf.Lerp(0.7f, 0f, t / flashOutDuration);
                flashImage.color = colorThisFrame;
                yield return null;
            }
        }
        flashImage.color = new Color32(0, 0, 0, 0);
    }
}
