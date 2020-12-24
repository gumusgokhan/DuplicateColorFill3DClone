using TMPro;
using UnityEngine;

// Responsible from fill count text in UI.
public class FillText : MonoBehaviour
{
    private TextMeshProUGUI fillText;
    private void Awake()
    {
        GameEvents.OnFillCountChange += OnFillCountChange;
        fillText = transform.GetComponent<TextMeshProUGUI>();
    }

    private void OnFillCountChange(int counter)
    {
        fillText.text = counter.ToString(); // when event is fired it updates UI text.
    }

    private void OnDestroy()
    {
        GameEvents.OnFillCountChange -= OnFillCountChange;
    }
}
