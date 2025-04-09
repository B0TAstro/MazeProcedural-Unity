using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze3 : MonoBehaviour
{
    public int width = 30;
    public int depth = 30;
    public GameObject cube;
    public GameObject startGO, endGO;
    private byte[,] map;

    private int xStart, zStart, xEnd, zEnd;
    private AStarPathfinding pathfinder;

    // Initialize maze dimensions and pathfinding component
    void Start()
    {
        width = (width % 2 == 0) ? width + 1 : width;
        depth = (depth % 2 == 0) ? depth + 1 : depth;

        pathfinder = FindObjectOfType<AStarPathfinding>();
        if (pathfinder == null)
        {
            Debug.LogError("AStarPathfinding component not found!");
            return;
        }

        GenerateNewMaze();
    }

    // Generate a new maze
    public void GenerateNewMaze()
    {
        CleanupScene();
        InitialiseMap();
        GenerateMaze(1, 1);
        Generate();
        CreateEntranceAndExit();
        DrawMap();
        StartPathfinding();
    }

    private List<GameObject> mazeElements = new List<GameObject>();

    // Clean up the scene by removing previous maze elements
    void CleanupScene()
    {
        foreach (GameObject obj in mazeElements)
        {
            if (obj != null)
                Destroy(obj);
        }
        mazeElements.Clear();
    }

    // Initialize the maze map with walls
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

    // Generate the maze using recursive backtracking
    void GenerateMaze(int x, int z)
    {
        map[x, z] = 0;

        int[] dx = { 0, 0, -2, 2 };
        int[] dz = { -2, 2, 0, 0 };

        int[] indices = { 0, 1, 2, 3 };
        for (int i = 0; i < indices.Length; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Length);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        foreach (int i in indices)
        {
            int newX = x + dx[i];
            int newZ = z + dz[i];

            if (newX > 0 && newX < width - 1 && newZ > 0 && newZ < depth - 1 && map[newX, newZ] == 1)
            {
                map[x + dx[i] / 2, z + dz[i] / 2] = 0;
                GenerateMaze(newX, newZ);
            }
        }
    }

    // Add random open spaces to the maze
    void Generate()
    {
        for (int z = 1; z < depth - 1; z++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (Random.Range(0, 100) < 5)
                {
                    map[x, z] = 0;
                }
            }
        }
    }

    // Create entrance and exit points for the maze
    void CreateEntranceAndExit()
    {
        if (Random.Range(0, 2) == 0)
        {
            xStart = 0;
            zStart = Random.Range(1, depth - 1);
        }
        else
        {
            xStart = width - 1;
            zStart = Random.Range(1, depth - 1);
        }

        if (Random.Range(0, 2) == 0)
        {
            xEnd = Random.Range(1, width - 1);
            zEnd = 0;
        }
        else
        {
            xEnd = Random.Range(1, width - 1);
            zEnd = depth - 1;
        }

        map[xStart, zStart] = 0;
        map[xEnd, zEnd] = 0;
    }

    // Draw the maze in the Unity scene
    void DrawMap()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] == 1)
                {
                    Vector3 pos = new Vector3(x, 1, z);
                    GameObject wallCube = Instantiate(cube, pos, Quaternion.identity);
                    mazeElements.Add(wallCube);
                }
            }
        }

        PlaceStartEnd();
    }

    // Place start and end markers in the maze
    void PlaceStartEnd()
    {
        Vector3 posStart = new Vector3(xStart, 1, zStart);
        GameObject start = Instantiate(startGO, posStart, Quaternion.identity);
        start.GetComponent<Renderer>().material.color = Color.green;
        mazeElements.Add(start);

        Vector3 posEnd = new Vector3(xEnd, 1, zEnd);
        GameObject end = Instantiate(endGO, posEnd, Quaternion.identity);
        end.GetComponent<Renderer>().material.color = Color.red;
        mazeElements.Add(end);
    }

    // Perform A* pathfinding to validate the maze
    void StartPathfinding()
    {
        bool[,] grid = new bool[width, depth];
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, z] = map[x, z] == 0;
            }
        }

        Vector2Int startPos = new Vector2Int(xStart, zStart);
        Vector2Int endPos = new Vector2Int(xEnd, zEnd);
        List<Vector2Int> path = pathfinder.FindPath(startPos, endPos, grid);

        if (path == null)
        {
            Debug.Log("Aucun chemin trouvé, régénération du labyrinthe...");
            GenerateNewMaze();
            return;
        }

        Debug.Log("Longueur du chemin: " + path.Count);

        if (path.Count < 10)
        {
            Debug.Log("Labyrinthe trop simple (chemin < 10), régénération...");
            GenerateNewMaze();
        }
        else
        {
            Debug.Log("Labyrinthe valide avec un chemin de longueur: " + path.Count);
        }
    }
}
