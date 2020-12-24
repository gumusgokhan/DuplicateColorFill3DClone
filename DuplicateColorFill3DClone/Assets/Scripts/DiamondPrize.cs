using UnityEngine;

public class DiamondPrize : MonoBehaviour
{
    //Responsible from diamond prize.

    private bool shouldMove;
    public GameObject DiamondIndicator;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private const float DistanceThreshold = 1.2f;
    public float moveSpeed;

    private void FixedUpdate()
    {
        if (!shouldMove)
            return;

        //Collected diamond moves towards the diamond shape on UI when collected. 
        if (Vector3.Distance(targetPosition, transform.position) < DistanceThreshold )
        {
            Destroy(gameObject);
        }

        transform.position += (targetPosition - startPosition) * moveSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("ReplicaCube"))
            return;

        transform.GetComponent<SphereCollider>().enabled = false; //disable collider to prevent further collisions.
        targetPosition = DiamondIndicator.transform.position;
        startPosition = transform.position;
        shouldMove = true;
        GameEvents.Instance.PrizeCollectionInvoker();
    }
}
