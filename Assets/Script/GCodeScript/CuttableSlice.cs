using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CuttableSlice : MonoBehaviour
{
    [HideInInspector] public bool isInside;

    public void DisableSlice()
    {
        gameObject.SetActive(false);
    }
}