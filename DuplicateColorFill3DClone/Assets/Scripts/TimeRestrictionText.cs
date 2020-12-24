using TMPro;
using UnityEngine;

/// <summary>
/// Responsible from updating timer text in Ui for bonus round.
/// </summary>

public class TimeRestrictionText : MonoBehaviour
{
    private TextMeshProUGUI timerText;

    private void Awake()
    {
        GameEvents.OnBonusRoundTimerChange += OnBonusRoundTimerChange;
        timerText = transform.GetComponent<TextMeshProUGUI>();
    }

    private void OnBonusRoundTimerChange(int counter)
    {
        timerText.text = counter.ToString();
    }


    private void OnDestroy()
    {
        GameEvents.OnBonusRoundTimerChange -= OnBonusRoundTimerChange;
    }
}
