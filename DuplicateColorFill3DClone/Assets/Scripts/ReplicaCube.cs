using UnityEngine;
/// <summary>
/// Responsible from Replica Cube's actions.
/// </summary>
public class ReplicaCube : MonoBehaviour
{
    [HideInInspector]
    public bool filled = false;
    public bool shouldHarm = false;
    public BoolVariable spawnCubesValOfPlayer;
     
    void Awake()
    {
        GameEvents.OnReplicaHit += OnReplicaHitMethod;
        GameEvents.OnWallHit += OnWallHit;
        filled = false;
    }

    private void OnReplicaHitMethod() // becomes a filled cube when player hit a filled replica cube.
    {
        if (!gameObject.activeSelf || filled)
            return;
        GameEvents.Instance.ChangeGridValueInvoker(Mathf.RoundToInt(transform.position.x) + 9, Mathf.Abs(Mathf.RoundToInt(transform.position.z) - 14));
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z);
        filled = true;
    }

    private void OnWallHit() // becomes a filled cube when player hit a wall.
    {
        if ( !gameObject.activeSelf || filled)
            return;
        GameEvents.Instance.ChangeGridValueInvoker(Mathf.RoundToInt(transform.position.x) + 9, Mathf.Abs(Mathf.RoundToInt(transform.position.z) - 14));
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z);
        filled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (filled)
            {
                GameEvents.Instance.ReplicaHitInvoker();
                if (spawnCubesValOfPlayer.value)
                {
                    GameEvents.Instance.FloodFillInvoker(); // Start flood fill
                }
                GameEvents.Instance.ReplicaAreaInvoker(false); // player is in safe area.
            }
            else
            {
                if (!shouldHarm)
                    return;
                GameEvents.Instance.PlayerDeathInvoker();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (filled)
            {
                GameEvents.Instance.ReplicaAreaInvoker(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (filled)
            {
                GameEvents.Instance.ReplicaAreaInvoker(true); // player left the safe area
                GameEvents.Instance.CubeLastSafeLocationInvoker(transform.position); //save last safe area location
            }
            else
            {
                shouldHarm = true; //unfilled cubes must start harming player (after put in the scene) when player left their collider. 
            }
        }
    }
    private void OnDestroy()
    {
        GameEvents.OnWallHit -= OnWallHit;
        GameEvents.OnReplicaHit -= OnReplicaHitMethod;
    }
}
