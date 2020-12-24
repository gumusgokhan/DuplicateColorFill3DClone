using UnityEngine;
// Responsible from Frame cubes which form a wall in the playfield.
public class FrameCube : MonoBehaviour
{
    public bool placedManually = false;
    void Start()
    {
        if (!placedManually) // If walls are generated from code instead of placing manually, they are already defined as true.
            return;          // Therefore invoking the event below is unnecessary.

        // Changes the value representing transform's location to true in the valueGrid located in PlayFieldManager.cs
        GameEvents.Instance.ChangeGridValueInvoker(Mathf.RoundToInt(transform.position.x) + 9, Mathf.Abs(Mathf.RoundToInt(transform.position.z) - 14));
    }

}
