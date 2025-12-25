using UnityEngine;
using System.Collections.Generic;

public class PlaneGridGenerator : MonoBehaviour
{
    public int gridX = 40;
    public int gridZ = 40;
    public float cellSize = 0.25f;
    public Material cellMaterial;

    public List<PlaneCell> cells = new List<PlaneCell>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.transform.parent = transform;
                cell.transform.rotation = Quaternion.Euler(90, 0, 0);
                cell.transform.localScale = Vector3.one * cellSize;

                Vector3 pos = new Vector3(
                    (x - gridX / 2f) * cellSize,
                    0,
                    (z - gridZ / 2f) * cellSize
                );

                cell.transform.localPosition = pos;

                if (cellMaterial)
                    cell.GetComponent<MeshRenderer>().material = cellMaterial;

                PlaneCell pc = cell.AddComponent<PlaneCell>();
                cells.Add(pc);
            }
        }
    }
}