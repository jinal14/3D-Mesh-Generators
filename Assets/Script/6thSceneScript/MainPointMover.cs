using System.Collections;
using UnityEngine;

public class MainPointMover : MonoBehaviour
{
    public Transform[] cubes;        // cube1, cube2, cube3, cube4
    public float moveSpeed = 0.8f;

    // Line Drawer Reference
    public LineDrawer lineDrawer;

    // Y height used for drawing
    public float drawY = 0.575f;

    // Final return position
    public Vector3 homePosition = new Vector3(0.222f, 0.6255f, -8.3594f);

    private void Start()
    {
        StartCoroutine(MovePath());
    }

    IEnumerator MovePath()
    {
        yield return new WaitForSeconds(1f);   // wait before starting

        for (int i = 0; i < cubes.Length; i++)
        {
            Transform target = cubes[i];

            // ----- MOVE X -----
            yield return MoveAxisTo(target.position.x, 'x');
            yield return new WaitForSeconds(1f);

            // ----- MOVE Z -----
            yield return MoveAxisTo(target.position.z, 'z');
            yield return new WaitForSeconds(1f);

            // ----- MOVE Y (forced draw height) -----
            yield return MoveAxisTo(drawY, 'y');

            // Start drawing only after reaching Cube1
            if (i == 0)
                lineDrawer.StartDrawing();

            yield return new WaitForSeconds(2f);  // wait after cube touched

            // Stop drawing after cube4
            if (i == cubes.Length - 1)
                lineDrawer.StopDrawing();
        }

        // AFTER LAST CUBE – RETURN HOME
        yield return new WaitForSeconds(1f);

        // Move X
        yield return MoveAxisTo(homePosition.x, 'x');
        yield return new WaitForSeconds(1f);

        // Move Z
        yield return MoveAxisTo(homePosition.z, 'z');
        yield return new WaitForSeconds(1f);

        // Move Y
        yield return MoveAxisTo(homePosition.y, 'y');
        yield return new WaitForSeconds(1f);

        Debug.Log("✔ MainPoint returned home successfully!");
    }

    // Smooth Axis Movement
    IEnumerator MoveAxisTo(float target, char axis)
    {
        while (true)
        {
            Vector3 pos = transform.position;

            if (axis == 'x')
                pos.x = Mathf.MoveTowards(pos.x, target, moveSpeed * Time.deltaTime);

            if (axis == 'z')
                pos.z = Mathf.MoveTowards(pos.z, target, moveSpeed * Time.deltaTime);

            if (axis == 'y')
                pos.y = Mathf.MoveTowards(pos.y, target, moveSpeed * Time.deltaTime);

            transform.position = pos;

            if (Mathf.Abs(GetAxisValue(axis) - target) < 0.0005f)
                break;

            yield return null;
        }
    }

    float GetAxisValue(char axis)
    {
        if (axis == 'x') return transform.position.x;
        if (axis == 'y') return transform.position.y;
        return transform.position.z;
    }
}
