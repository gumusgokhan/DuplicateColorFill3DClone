using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContinueScreen : MonoBehaviour
{
    //Responsible from continue screen.

    public int countdownDuration;
    public GameObject countdownTextObject;
    public IntVariable prizeCount;
    private TextMeshProUGUI countdownText;

    void OnEnable()
    {
        countdownText = countdownTextObject.GetComponent<TextMeshProUGUI>();
        if (countdownText == null)
            return;
        countdownText.text = countdownDuration.ToString();
        StartCoroutine(CountDown()); // Countdown coroutine. Currently it is set to 10 seconds. It can be changed from inspector.
    }
    public void NoThanksButtonFunc()
    {
        GameEvents.Instance.NoThanksInvoker();
    }

    public void ContinueButtonFunc()
    {
        if(prizeCount.value < 5)
        {
            // TODO: Activate Store UI
            return;
        }
        prizeCount.value -= 5;
        GameEvents.Instance.ContinueInvoker();
    }

    private IEnumerator CountDown()
    {
        int counter = countdownDuration;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
            countdownText.text = counter.ToString();
        }

        NoThanksButtonFunc();
    }
}
