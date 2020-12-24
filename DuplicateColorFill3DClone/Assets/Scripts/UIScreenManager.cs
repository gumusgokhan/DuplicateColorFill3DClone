using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible from managing all UI screens. Currently it only manages the continue screen.
/// </summary>
public class UIScreenManager : MonoBehaviour
{
    public GameObject ContinueScreen;
    private void Awake()
    {
        GameEvents.OnPlayerDeathCompleted += OnPlayerDeathCompleted;
    }

    private void OnPlayerDeathCompleted() // Activates continue screen after player death.
    {
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            GameEvents.Instance.BonusRoundCompleteInvoker(); // Does not activate continue screen if player dies in bonus round.
            return;
        }

        if(!ContinueScreen.activeSelf)
            ContinueScreen.SetActive(true);
    }

    private void OnDestroy()
    {
        GameEvents.OnPlayerDeathCompleted -= OnPlayerDeathCompleted;
    }
}
