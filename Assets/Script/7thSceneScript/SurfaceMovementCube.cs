using System.Collections;
using UnityEngine;

public class SurfaceMovementCube : MonoBehaviour
{
    public Transform surface;

    public Transform cube1;
    public Transform cube2;
    public Transform cube3;
    public Transform cube4;

    public float moveSpeed = 2f;

    float bottomY = 0.52f;   // Starting Y level
    float topY = 0.70f;      // Cube activation Y level

    void Start()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        // Start position
        surface.position = new Vector3(0.21f, bottomY, -8.36f);
        yield return new WaitForSeconds(1);

        // --- CUBE 1 ---
        yield return MoveToCube(cube1, cube1.gameObject);

        // --- CUBE 2 ---
        yield return MoveToCube(cube2, cube2.gameObject);

        // --- CUBE 3 ---
        yield return MoveToCube(cube3, cube3.gameObject);

        // --- CUBE 4 ---
        yield return MoveToCube(cube4, cube4.gameObject);

        // --- RETURN HOME ---
        yield return MoveAxisTo(0.21f, 'x');
        yield return new WaitForSeconds(1);

        yield return MoveAxisTo(-8.36f, 'z');
        yield return new WaitForSeconds(1);

        yield return MoveAxisTo(bottomY, 'y');
        yield return new WaitForSeconds(1);

        Debug.Log("✔ Movement Completed Successfully!");
    }

    IEnumerator MoveToCube(Transform cube, GameObject cubeObj)
    {
        // Move X to cube
        yield return MoveAxisTo(cube.position.x, 'x');
        yield return new WaitForSeconds(1);

        // Move Z to cube
        yield return MoveAxisTo(cube.position.z, 'z');
        yield return new WaitForSeconds(1);

        // Move Y UP and activate cube
        yield return MoveAxisTo(topY, 'y');
        cubeObj.SetActive(true);
        yield return new WaitForSeconds(2);

        // Move Y DOWN
        yield return MoveAxisTo(bottomY, 'y');
        yield return new WaitForSeconds(1);
    }

    // Move one axis only
    IEnumerator MoveAxisTo(float target, char axis)
    {
        while (true)
        {
            Vector3 pos = surface.position;

            if (axis == 'x')
                pos.x = Mathf.MoveTowards(pos.x, target, moveSpeed * Time.deltaTime);
            else if (axis == 'z')
                pos.z = Mathf.MoveTowards(pos.z, target, moveSpeed * Time.deltaTime);
            else if (axis == 'y')
                pos.y = Mathf.MoveTowards(pos.y, target, moveSpeed * Time.deltaTime);

            surface.position = pos;

            if (Mathf.Abs(GetAxis(axis) - target) < 0.0005f)
                break;

            yield return null;
        }
    }

    float GetAxis(char axis)
    {
        if (axis == 'x') return surface.position.x;
        if (axis == 'y') return surface.position.y;
        return surface.position.z;
    }
}
