using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Responsible from enemy cubes' coloring and trigger operations.
public class EnemyCube : MonoBehaviour
{

    [SerializeField]
    private List<Color> colorList;
    [SerializeField]
    private Material enemyCubeMaterial;

    [SerializeField]
    private ParticleSystem particleSystem;

    public BoolVariable spawnCubesValOfPlayer;
    private void Start()
    {
        Color color = colorList[Random.Range(0, colorList.Count)]; //cubes' color is randomly selected from a list defined in inspector.
        enemyCubeMaterial.color = color;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "ReplicaCube":
                if (other.transform.GetComponent<ReplicaCube>().filled)
                {
                    transform.GetComponent<BoxCollider>().enabled = false;
                    transform.GetComponent<MeshRenderer>().enabled = false;
                    particleSystem.Play();
                    //Handheld.Vibrate();
                    Destroy(this.gameObject, 1.0f);
                }
                else
                {
                    GameEvents.Instance.PlayerDeathInvoker(); // if enemy cube hits player's tail player dies.
                }
                break;
            case "Player":
                if (!spawnCubesValOfPlayer.value) // if player does not spawn replica cubes which means it's in safe area, enemy cube does not harm player.
                    return;
                GameEvents.Instance.PlayerDeathInvoker(); // if player directly hits enemy cube player dies.
                break;
            default:
                break;
        }
    }
}
