using UnityEngine;

public enum moveDirection
{
    Horizontal,
    Vertical
};

//Responsible from Enemy Cluster's movement and rotation.
public class EnemyCluster : MonoBehaviour
{
    public bool moveAround;
    public moveDirection moveDirection;
    public float maxPoint;
    public float minPoint;
    public float moveSpeed;
    private Vector3 targetPosition;
    private Vector3 startPosition;

    public bool rotateAround;
    [Range(-1.0f, 1.0f)]
    public float yForce;
    public float rotationSpeed;

    private void Start()
    {
        startPosition = transform.position;                             

        switch (moveDirection)
        {
            case moveDirection.Horizontal:
                Vector3 xMin = new Vector3(minPoint, 0.0f, 0.0f);
                targetPosition = xMin;
                break;
            case moveDirection.Vertical:
                Vector3 zMin = new Vector3(0.0f, 0.0f, minPoint);
                targetPosition = zMin;
                break;
            default:
                break;
        }
    }
    
    // Movement and rotation operations.
    void FixedUpdate()
    {
        if (!moveAround && !rotateAround)
            return;

        if (moveAround)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);


            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {

                switch (moveDirection)
                {
                    case moveDirection.Horizontal:
                        Vector3 xMin = new Vector3(minPoint, 0.0f, 0.0f);
                        if (targetPosition.Equals(xMin))
                        {
                            Vector3 xMax = new Vector3(maxPoint, 0.0f, 0.0f);
                            targetPosition = xMax;
                        }
                        else
                            targetPosition = xMin;
                        break;
                    case moveDirection.Vertical:
                        Vector3 zMin = new Vector3(0.0f, 0.0f, minPoint);
                        if (targetPosition.Equals(zMin))
                        {
                            Vector3 zMax = new Vector3(0.0f, 0.0f, maxPoint);
                            targetPosition = zMax;
                        }
                        else
                            targetPosition = zMin;
                        break;
                    default:
                        break;
                }
            }
        }

        if (!rotateAround)
            return;

        Vector3 eulerVector = new Vector3(0.0f, yForce, 0.0f);
        transform.Rotate(eulerVector * rotationSpeed);
    }
}
