using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Responsible from players' movement and collisions.
public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
     
    [HideInInspector]
    public bool moving;
    public BoolVariable spawnCubes;

    private bool deathStarted;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 moveVector;
    private Vector3 playerLastSafeCubeLocation;

    private Rigidbody rb;
    [SerializeField]
    private List<Color> colorList;
    [SerializeField]
    private Material playerMaterial;
    [SerializeField]
    private Material replicaMaterial;

    [SerializeField]
    private GameObject cubePrefab;

    [SerializeField]
    private ParticleSystem particleSystem;

    [SerializeField]
    private ColorVariable color;

    public ReplicaCubeObjectPool replicaCubeObjectPool;
    private List<ReplicaCube> spawnedCubesList;
    
    private const float DistanceThreshold = 1.0f;
    private const float MaxSwipeTime = 0.5f;
    private const float MinSwipeDistance = 0.10f;

    private bool swipedRight = false;
    private bool swipedLeft = false;
    private bool swipedUp = false;
    private bool swipedDown = false;


    public bool useKeyboardKeys = true;

    Vector2 startPos;
    float startTime;

    private void Awake()
    {
        GameEvents.OnReplicaArea += OnReplicaArea;
        GameEvents.OnPlayerDeath += OnPlayerDeath;
        GameEvents.OnCubeLastSafeLocation += OnCubeLastSafeLocation;
        GameEvents.OnContinue += OnContinue;
        GameEvents.OnSpeedPowerUp += OnSpeedPowerUp;
        color.value = colorList[Random.Range(0, colorList.Count)];
        playerMaterial.color = color.value;
        replicaMaterial.color = color.value * 2f; // apply lighter color to replica cubes.
    }

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        spawnedCubesList = new List<ReplicaCube>();
        //Spawn a cube in starting point.
        ReplicaCube replicaAtStart = replicaCubeObjectPool.GetObjectFromPool(new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z), Quaternion.identity);
        replicaAtStart.filled = true;
        GameEvents.Instance.ChangeGridValueInvoker(Mathf.RoundToInt(transform.position.x) + 9, Mathf.Abs(Mathf.RoundToInt(transform.position.z) - 14));
    }

    void Update()
    {
        if (deathStarted)
            return;
        DetectSwipe();
        ApplyMovementDecision();
    }

    /// <summary>
    /// Function to detect swipe movement. It decides direction of swipe.
    /// </summary>
    private void DetectSwipe()
    {
        swipedRight = false;
        swipedLeft = false;
        swipedUp = false;
        swipedDown = false;

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                startPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width); // normalize position according to screen width.
                startTime = Time.time;
            }
            if (t.phase == TouchPhase.Ended)
            {
                if (Time.time - startTime > MaxSwipeTime) // swipe duration is too short.
                    return;

                Vector2 endPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);

                Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

                if (swipe.magnitude < MinSwipeDistance) // swipe magnitude is below threshold.
                    return;

                if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                { 
                    if (swipe.x > 0)
                    {
                        swipedRight = true;
                    }
                    else
                    {
                        swipedLeft = true;
                    }
                }
                else
                { 
                    if (swipe.y > 0)
                    {
                        swipedUp = true;
                    }
                    else
                    {
                        swipedDown = true;
                    }
                }
            }
        }

        if (useKeyboardKeys) // to debug using keyboard keys while working on a PC.
        {
            swipedDown = swipedDown || Input.GetKeyDown(KeyCode.S);
            swipedUp = swipedUp || Input.GetKeyDown(KeyCode.W);
            swipedRight = swipedRight || Input.GetKeyDown(KeyCode.D);
            swipedLeft = swipedLeft || Input.GetKeyDown(KeyCode.A);
        }
    }
    
    /// <summary>
    /// Function to assing a movement vector to player cube according to swipe direction.
    /// </summary>
    private void ApplyMovementDecision()
    {
        if (swipedUp)
        {
            if (spawnCubes.value && moveVector.Equals(Vector3.back)) //player can not move in reverse direction while spawning cubes.
                return;
            moveVector = Vector3.forward;
            SetTargetPosAndProperties();
        }
        else if (swipedDown)
        {
            if (spawnCubes.value && moveVector.Equals(Vector3.forward))
                return;
            moveVector = Vector3.back;
            SetTargetPosAndProperties();
        }
        else if (swipedLeft)
        {
            if (spawnCubes.value && moveVector.Equals(Vector3.right))
                return;
            moveVector = Vector3.left;
            SetTargetPosAndProperties();
        }
        else if (swipedRight)
        {
            if (spawnCubes.value && moveVector.Equals(Vector3.left))
                return;
            moveVector = Vector3.right;
            SetTargetPosAndProperties();
        }
    }
    private void SetTargetPosAndProperties()
    {
        if (moving)
            return;
        targetPosition = transform.position + moveVector;
        startPosition = transform.position;
        moving = true;
        spawnCubes.value = true;
    }
    private void FixedUpdate()
    {
        if (moving)
        {
            if (Vector3.Distance(startPosition, transform.position) > DistanceThreshold)
            {
                transform.position = targetPosition;
                startPosition = transform.position;
                targetPosition = transform.position + moveVector;
                if (spawnCubes.value) //get cubes from object pool and spawn them while moving.
                {
                    var repCube = replicaCubeObjectPool.GetObjectFromPool(new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z), Quaternion.identity);
                    spawnedCubesList.Add(repCube); // list is kept to clean unfilled cubes if player dies.
                }
            }

            transform.position += (targetPosition - startPosition) * moveSpeed * Time.deltaTime;
        }
    }

    private void OnPlayerDeath()
    {
        if (deathStarted)
            return;
        deathStarted = true;
        moving = false;
        spawnCubes.value = false;
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<MeshRenderer>().enabled = false;
        particleSystem.Play();       
        StartCoroutine(DeactivateUnfilledCubes());
    }

    private void OnSpeedPowerUp(float multiplier, string type)
    {
        moveSpeed = moveSpeed * multiplier;
    }

    /// <summary>
    /// Can be utilized for immediate clearance of unfilled cubes
    /// </summary>
    private void DeactivateReplicaCubes()
    {
        foreach (ReplicaCube cube in spawnedCubesList)
        {
            replicaCubeObjectPool.DeactivateReplicaCube(cube);
        }
    }

    /// <summary>
    /// If player choose to continue player cube will be put back to the scene.
    /// </summary>
    private void OnContinue()
    {
        deathStarted = false;
        moving = false;
        spawnCubes.value = false;
        transform.position = new Vector3(playerLastSafeCubeLocation.x, 0.5f, playerLastSafeCubeLocation.z);
        transform.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<MeshRenderer>().enabled = true;
        gameObject.SetActive(true);
    }

    private void OnReplicaArea(bool cubeSpawnDecision)
    {
        spawnCubes.value = cubeSpawnDecision;
        if (!cubeSpawnDecision && spawnedCubesList.Count > 0) // clear unfilled cube list when player enters safe area.
            spawnedCubesList.Clear();
    }
    private void OnCubeLastSafeLocation(Vector3 lastSafePos)
    {
        playerLastSafeCubeLocation = lastSafePos; // last safe location is kept to put back player to the scene when player choose to continue.
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "FrameCube")
        {
            moving = false;
            rb.velocity = Vector3.zero;
            moveVector = Vector3.zero;
            transform.position = new Vector3((float)Mathf.Round(transform.position.x), transform.position.y, (float)Mathf.Round(transform.position.z));
            if (spawnCubes.value)
            {
                GameEvents.Instance.WallHitInvoker(); // Fill unfilled cubes
                spawnCubes.value = false;
                spawnedCubesList.Clear();
                GameEvents.Instance.FloodFillInvoker(); // Start flood fill process.
            }
        }

    }

    /// <summary>
    /// Creating simple effect while clearing unfilled cubes
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeactivateUnfilledCubes()
    {
        foreach (ReplicaCube cube in spawnedCubesList)
        {
            yield return new WaitForSeconds(0.05f);
            replicaCubeObjectPool.DeactivateReplicaCube(cube);
        }
        GameEvents.Instance.PlayerDeathCompletedInvoker();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.OnReplicaArea -= OnReplicaArea;
        GameEvents.OnPlayerDeath -= OnPlayerDeath;
        GameEvents.OnCubeLastSafeLocation -= OnCubeLastSafeLocation;
        GameEvents.OnContinue -= OnContinue;
        GameEvents.OnSpeedPowerUp -= OnSpeedPowerUp;
    }
}
