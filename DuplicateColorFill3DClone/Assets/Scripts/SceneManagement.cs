using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible from scene management.
/// </summary>

public class SceneManagement : MonoBehaviour
{
    public IntVariable latestLevelIndex;
    private int bonusRoundIndex;
    private int activeSceneIndex;
    private int fillCountLimitForBonusRound = 10;
    private void Awake()
    {
        GameEvents.OnNoThanks += OnNoThanks;
        GameEvents.OnLevelComplete += OnLevelComplete;
        GameEvents.OnBonusRoundComplete += OnBonusRoundCompleted;
    }

    private void Start()
    {
        bonusRoundIndex = SceneManager.sceneCountInBuildSettings-1; // bonus round is set as the last scene in build setting.
        activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (activeSceneIndex != bonusRoundIndex) // if active scene is not bonus round its index is saved to be used after possible bonus round
        {
            latestLevelIndex.value = activeSceneIndex;
        }
    }

    private void OnBonusRoundCompleted() //bonus round completion
    {
        StartCoroutine(LoadNextLevel());
    }

    private void OnLevelComplete(int fillCount) //normal level completion. fillCount is utilized to decide load bonus round or not.
    {
        StartCoroutine(LoadNextLevel(fillCount));
    }

    private void OnNoThanks() //not continue decision
    {
        if (activeSceneIndex != bonusRoundIndex)
            SceneManager.LoadScene(activeSceneIndex);
    }

    IEnumerator LoadNextLevel(int fillCount = 20) //default value is set for empty calls
    {
        yield return new WaitForSeconds(1);

        DecideSceneToLoad(fillCount);
    }

    private void DecideSceneToLoad(int fillCount)
    {
        // if player is not in bonus round and fill count is <= limit (currently 10 fills) load bonus round.
        if (activeSceneIndex != bonusRoundIndex && fillCount <= fillCountLimitForBonusRound)
        {
            SceneManager.LoadScene(bonusRoundIndex);
            return;
        }

        int nextSceneBuildIndex;

        if (activeSceneIndex == bonusRoundIndex) //continue normal levels after bonus round
            nextSceneBuildIndex = latestLevelIndex.value + 1;
        else
            nextSceneBuildIndex = activeSceneIndex + 1;

        if (nextSceneBuildIndex != bonusRoundIndex && nextSceneBuildIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneBuildIndex);
        else
            SceneManager.LoadScene(0);
    }

    //IEnumerator RestartLevel()
    //{
    //    yield return new WaitForSeconds(2);
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //}

    private void OnDisable()
    {
        GameEvents.OnNoThanks -= OnNoThanks;
        GameEvents.OnLevelComplete -= OnLevelComplete;
        GameEvents.OnBonusRoundComplete -= OnBonusRoundCompleted;

    }
}
