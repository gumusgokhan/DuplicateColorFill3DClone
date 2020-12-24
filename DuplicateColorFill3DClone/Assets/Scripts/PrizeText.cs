using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Responsible from updating prize text and count. 
/// </summary>
public class PrizeText : MonoBehaviour
{
    [SerializeField]
    private IntVariable prizeCount;
    private TextMeshProUGUI prizeCountText;
    private int prizeCountInALevel;
    private int prizeWorth;

    private void Awake()
    {
        GameEvents.OnPrizeCollection += OnPrizeCollection;       
        GameEvents.OnContinue += OnContinue;       
        GameEvents.OnNoThanks += OnNoThanks;       
        GameEvents.OnDiamondPowerUp += OnDiamondPowerUp;       
    }

    private void Start()
    {
        prizeCountText = transform.GetComponent<TextMeshProUGUI>();
        prizeCountText.text = prizeCount.value.ToString();
        prizeCountInALevel = 0;
        prizeWorth = 1;
    }

    private void OnDiamondPowerUp(int multiplier, string type)
    {
        prizeWorth = prizeWorth * multiplier;
    }

    private void OnContinue()
    {
        UpdatePrizeText();
    }

    private void OnNoThanks()
    {
        prizeCount.value -= prizeCountInALevel; // if player choose not to continue get back the diamonds which are collected in that level.
        prizeCountInALevel = 0;
        UpdatePrizeText();
    }

    private void OnPrizeCollection()
    {
        prizeCount.value += prizeWorth;
        prizeCountInALevel += prizeWorth;
        UpdatePrizeText();
    }

    private void UpdatePrizeText()
    {
        prizeCountText.text = prizeCount.value.ToString();
        ForceSerialization();
    }

    private void OnDestroy()
    {
        GameEvents.OnPrizeCollection -= OnPrizeCollection;
        GameEvents.OnContinue -= OnContinue;
        GameEvents.OnNoThanks -= OnNoThanks;
    }

    /// <summary>
    /// Responsible from preventing scriptable object reset when Unity editor is closed.
    /// </summary>
    void ForceSerialization()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(prizeCount);
        #endif
    }
}
