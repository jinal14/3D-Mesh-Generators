using UnityEngine;

public class lineTesting : MonoBehaviour
{
    public Transform surface;       // the cube (or drawing point)
    public DrawLine line;

    private void Start()
    {
        line.StartLine(surface);
    }
}