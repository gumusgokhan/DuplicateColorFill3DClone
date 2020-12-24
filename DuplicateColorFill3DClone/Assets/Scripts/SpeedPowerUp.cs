using UnityEngine;
/// <summary>
/// Responsible from speed powerup collision.
/// </summary>
public class SpeedPowerUp : MonoBehaviour
{
    public float speedMultiplier;
    //public float powerUpDuration;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("ReplicaCube"))
            return;

        GameEvents.Instance.SpeedPowerUpInvoker(speedMultiplier, "Speed");
        Destroy(gameObject);
    }

}
