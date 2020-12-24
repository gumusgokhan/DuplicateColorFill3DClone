using TMPro;
using UnityEngine;
/// <summary>
/// Responsible from updating powerup text in UI.
/// </summary>
public class PowerUpText : MonoBehaviour
{
    private TextMeshProUGUI powerUpTextBox;
    private string powerUpText;

    private void Awake()
    {
        GameEvents.OnSpeedPowerUp += OnSpeedPowerUp;
        GameEvents.OnDiamondPowerUp += OnDiamondPowerUp;
        powerUpTextBox = transform.GetComponent<TextMeshProUGUI>();
    }

    private void OnDiamondPowerUp(int multiplier, string type)
    {
        powerUpText += $" {multiplier}x {type} \n";
        powerUpTextBox.text = powerUpText;
    }

    private void OnSpeedPowerUp(float multiplier, string type)
    {
        powerUpText += $" {multiplier}x {type} \n";
        powerUpTextBox.text = powerUpText;
    }

    private void OnDestroy()
    {
        GameEvents.OnSpeedPowerUp -= OnSpeedPowerUp;
        GameEvents.OnDiamondPowerUp -= OnDiamondPowerUp;
    }
}
