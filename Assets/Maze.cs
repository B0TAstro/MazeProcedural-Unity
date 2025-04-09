using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public GameObject cube;
    public int width = 30;
    public int depth = 30;

    void Start()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                Instantiate(cube, pos, Quaternion.identity);
            }
        }
    }

    void Update()
    {

    }
}