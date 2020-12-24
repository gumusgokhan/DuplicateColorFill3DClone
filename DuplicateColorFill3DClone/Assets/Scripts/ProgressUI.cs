using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    // Responsible from updating level progress bar.

    public FloatVariable levelProgressPercentage;
    public ColorVariable color;
    private Image progressImage;

    private void Awake()
    {
        GameEvents.OnProgressChange += OnProgressChange;
        levelProgressPercentage.value = 0f;
    }

    private void Start()
    {
        progressImage = transform.GetComponent<Image>();
        progressImage.color = color.value;
        progressImage.fillAmount = levelProgressPercentage.value;
    }

    private void OnProgressChange()
    {
        progressImage.fillAmount = levelProgressPercentage.value; //change fill amount when event is fired. Value is acquired from a scriptable object.
    }

    private void OnDestroy()
    {
        GameEvents.OnProgressChange -= OnProgressChange;
    }
}
