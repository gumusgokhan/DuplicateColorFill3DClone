using UnityEngine;

public class DiamondPowerUp : MonoBehaviour
{
    public int diamondMultiplier;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("ReplicaCube"))
            return;

        GameEvents.Instance.DiamondPowerUpInvoker(diamondMultiplier, "Diamond"); //fire the event when diamond powerup is collected.
        Destroy(gameObject);
    }
}
