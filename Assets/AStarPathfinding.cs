using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AStarPathfinding : MonoBehaviour
{
    // Node class representing a single point in the grid
    public class Node
    {
        public Vector2Int Position;
        public Node Parent;
        public int G;
        public int H;
        public int F => G + H;

        public Node(Vector2Int position, Node parent = null)
        {
            Position = position;
            Parent = parent;
            G = parent != null ? parent.G + 1 : 0;
            H = 0;
        }
    }

    // List to store path visualization objects
    private List<GameObject> pathObjects = new List<GameObject>();

    // Main method to find the path using A* algorithm
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, bool[,] grid)
    {
        ClearPathObjects();
        
        List<Node> openSet = new List<Node>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        List<Vector2Int> exployellow = new List<Vector2Int>();
        
        Node startNode = new Node(start);
        startNode.H = ManhattanDistance(start, goal);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => a.F.CompareTo(b.F));
            Node current = openSet[0];
            openSet.RemoveAt(0);

            if (current.Position == goal)
            {
                List<Vector2Int> path = ReconstructPath(current);
                StartCoroutine(DisplayPathAnimation(path, exployellow));
                return path;
            }

            closedSet.Add(current.Position);
            exployellow.Add(current.Position);

            foreach (Vector2Int direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int neighborPos = current.Position + direction;
                
                if (!IsValid(neighborPos, grid) || closedSet.Contains(neighborPos))
                    continue;

                Node neighbor = new Node(neighborPos, current);
                neighbor.H = ManhattanDistance(neighborPos, goal);

                if (!openSet.Exists(n => n.Position == neighborPos && n.F <= neighbor.F))
                    openSet.Add(neighbor);
            }
        }
        
        Debug.Log("No path found from " + start + " to " + goal);
        return null;
    }

    // Reconstructs the path from the goal node to the start node
    private List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (node != null)
        {
            path.Add(node.Position);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }

    // Validates if a position is within bounds and walkable
    private bool IsValid(Vector2Int pos, bool[,] grid)
    {
        return pos.x >= 0 && pos.y >= 0 && 
               pos.x < grid.GetLength(0) && pos.y < grid.GetLength(1) && 
               grid[pos.x, pos.y];
    }

    // Calculates the Manhattan distance between two points
    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Clears previously created path visualization objects
    public void ClearPathObjects()
    {
        foreach (GameObject obj in pathObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        pathObjects.Clear();
    }

    // Coroutine to animate the pathfinding process
    private IEnumerator DisplayPathAnimation(List<Vector2Int> path, List<Vector2Int> exployellow)
    {
        HashSet<Vector2Int> pathSet = new HashSet<Vector2Int>(path);
        Dictionary<Vector2Int, GameObject> exployellowCubes = new Dictionary<Vector2Int, GameObject>();
        
        for (int i = 0; i < exployellow.Count; i++)
        {
            Vector2Int position = exployellow[i];
            Vector3 worldPosition = new Vector3(position.x, 0.5f, position.y);
            GameObject exploreCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            exploreCube.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
            exploreCube.transform.position = worldPosition;
            exploreCube.tag = "MazeElement";
            
            Renderer renderer = exploreCube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.yellow;
            }
            
            pathObjects.Add(exploreCube);
            exployellowCubes[position] = exploreCube;
            
            if (position.Equals(path[path.Count - 1]))
            {
                break;
            }
            
            yield return new WaitForSeconds(0.08f);
        }
        
        yield return new WaitForSeconds(1.0f);
        
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int position = path[i];
            
            if (exployellowCubes.ContainsKey(position))
            {
                GameObject cube = exployellowCubes[position];
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    StartCoroutine(ChangeColorGradually(renderer, Color.yellow, Color.green, 0.5f));
                    StartCoroutine(ChangeScaleGradually(cube.transform, 
                        new Vector3(0.8f, 0.1f, 0.8f), 
                        new Vector3(0.8f, 0.2f, 0.8f), 
                        0.5f));
                }
            }
            else
            {
                Vector3 worldPosition = new Vector3(position.x, 0.5f, position.y);
                GameObject pathCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pathCube.transform.localScale = new Vector3(0.8f, 0.2f, 0.8f);
                pathCube.transform.position = worldPosition;
                pathCube.tag = "MazeElement";
                
                Renderer renderer = pathCube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.green;
                }
                
                pathObjects.Add(pathCube);
            }
            
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Coroutine to gradually change the color of an object
    private IEnumerator ChangeColorGradually(Renderer renderer, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            renderer.material.color = Color.Lerp(startColor, endColor, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        renderer.material.color = endColor;
    }

    // Coroutine to gradually change the scale of an object
    private IEnumerator ChangeScaleGradually(Transform transform, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = endScale;
    }

    // Registers a path object for visualization
    public void RegisterPathObject(GameObject obj, List<GameObject> registry)
    {
        pathObjects.Add(obj);
        registry.Add(obj);
    }

    // Displays the path without animation
    public void DisplayPath(List<Vector2Int> path)
    {
        if (path == null)
            return;
            
        foreach (var position in path)
        {
            Vector3 worldPosition = new Vector3(position.x, 0.5f, position.y);
            GameObject pathCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pathCube.transform.localScale = new Vector3(0.8f, 0.2f, 0.8f);
            pathCube.transform.position = worldPosition;
            pathCube.tag = "MazeElement";
            
            Renderer renderer = pathCube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
            }
            
            pathObjects.Add(pathCube);
        }
    }
}
