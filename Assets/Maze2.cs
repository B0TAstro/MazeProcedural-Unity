using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze2 : MonoBehaviour
{
    public int width = 30;
    public int depth = 30;
    public byte[,] map;
    void Start()
    {
        InitialiseMap();
        //Generate();
        Generate2();
        DrawMap();
    }
    void InitialiseMap()
    {
        map = new byte[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;
            }
        }
    }
    // void Generate()
    // {
    //     for (int z = 0; z < depth; z++)
    //     {
    //         for (int x = 0; x < width; x++)
    //         {
    //             if(Random.Range(0,100) < 50)
    //             {
    //                 map[x, z] = 0;
    //             }
    //         }
    //     }
    // }
    void Generate2(){
        bool done = false;
        int x = width / 2;
        int z = depth / 2;

        while (!done) {
            map[x, z] = 0;
            int alea = Random.Range(-1, 2);
            x += alea;
            if (alea == 0) {
                z += Random.Range(-1, 2);
            }
            if (x < 0 || x >= width || z < 0 || z >= depth) {
                done = true;
            } else {
                done = false;
            }
        }
    }
    void DrawMap() {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if(map[x,z] == 1)
                {
                    Vector3 pos = new Vector3(x, 1, z);
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.transform.position = pos;
                }
            }
        }
    }
    void Update()
    {
        
    }
}
