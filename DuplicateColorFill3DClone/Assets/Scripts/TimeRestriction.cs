using System.Collections;
using UnityEngine;
/// <summary>
/// Responsible from managing time restriction (currently set to 10 seconds) in Bonus round.
/// </summary>
public class TimeRestriction : MonoBehaviour
{
    public int countdownDuration;
    private GameEvents gameEvents;

    void Start()
    {
        gameEvents = GameEvents.Instance;
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        int counter = countdownDuration;
        gameEvents.BonusRoundTimerChangeInvoker(counter);
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
            gameEvents.BonusRoundTimerChangeInvoker(counter); //Fire an event to update countdown timer text in UI
        }

        GameEvents.Instance.BonusRoundCompleteInvoker(); // Fire an event after time is up
    }
}
