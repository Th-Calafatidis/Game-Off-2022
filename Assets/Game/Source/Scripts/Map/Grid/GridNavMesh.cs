// /* --------------------
// -----------------------
// Creation date: 03/11/2022
// Author: Alex
// Description: This is a class to store information about where units will be able to move
//              on a grid.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNavMesh
{
    // List of all nodes on the navmesh.
    private Node[,] m_nodes;

    // The size of the navmesh.
    private Vector2 m_size;

    // Offsets to use for converting world positions to grid positions.
    private int m_xOffset;
    private int m_yOffset;

    // References to the gameobjects that will be used to generate the navmesh.
    private Transform m_groundHolder;
    private List<Transform> m_obstacleHolders;
    private HashSet<Vector2Int> m_groundPositions;

    public GridNavMesh(Transform groundHolder, List<Transform> obstacleHolders, bool bakeMesh = false)
    {
        m_groundHolder = groundHolder;
        m_obstacleHolders = obstacleHolders;
        if (bakeMesh) Bake();
    }

    public Node[,] Nodes
    {
        get { return m_nodes; }
    }

    /// <summary>
    /// The width of the navmesh
    /// </summary>
    public int Width
    {
        get { return (int)m_size.x; }
    }

    /// <summary>
    /// The height of the navmesh.
    /// </summary>
    public int Height
    {
        get { return (int)m_size.y; }
    }

    #region Public Methods

    /// <summary>
    /// Generates a new navmesh with obstacles and ground.
    /// </summary>
    public void Bake()
    {
        Debug.Log("owo3");
        // We include an empty list of the additional to make new system happy.
        List<Vector2Int> additionalObstacles = new List<Vector2Int>();
        InternalBake(GetObstaclePositions(), additionalObstacles);
    }

    /// <summary>
    /// Generates a new navmesh where you can add additional points
    /// that will be considered obstructed. This is one entry into the 
    /// long running series I like to call "Alex's Fantastical Hacks", and is
    /// used for enemy navigation when there are hazards on the grid.
    /// </summary>
    /// <param name="additionalObsticles"></param>
    public void Bake(List<Vector2Int> additionalObsticles)
    {
        Debug.Log("owo2");
        InternalBake(GetObstaclePositions(), additionalObsticles);
    }

    /// <summary>
    /// Generates a new navmesh that purposely ignores the obstacles.
    /// </summary>
    /// <param name="ignoredTiles"></param>
    public void BakeIgnored(List<Vector2Int> ignoredTiles)
    {
        Debug.Log("owo");
        List<Vector2Int> obstacles = GetObstaclePositions();
        // remove ignored tiles from obstacles
        foreach (Vector2Int ignoredTile in ignoredTiles)
        {
            obstacles.Remove(ignoredTile);
        }
        List<Vector2Int> additionalObstacles = new List<Vector2Int>();
        InternalBake(obstacles, additionalObstacles);
    }

    /// <summary>
    /// Returns the node at the given x and y.
    /// Will return null if out of bounds.
    /// </summary>
    /// <param name="x">X position on grid</param>
    /// <param name="y">Y position on grid</param>
    /// <returns></returns>
    public Node GetNodeAt(int x, int y)
    {
        if (x < 0 || x >= m_size.x || y < 0 || y >= m_size.y)
        {
            return null;
        }

        return m_nodes[x, y];
    }

    /// <summary>
    /// Returns the node at the given world position.
    /// Will return null if out of bounds.
    /// </summary>
    /// <param name="worldPosition">Position in world space</param>
    /// <returns></returns>
    public Node GetNodeAt(Vector3 worldPosition)
    {
        // Convert X and Z to grid positions. Add 0.5f because of origin point.
        int x = (worldPosition.x > 0) ? Mathf.FloorToInt(worldPosition.x + 0.5f) : Mathf.CeilToInt(worldPosition.x - 0.5f);
        int y = worldPosition.z > 0 ? Mathf.FloorToInt(-worldPosition.z + 0.5f) : Mathf.CeilToInt(-worldPosition.z - 0.5f);

        // Since Y will be inverted, we subtract the value from size to an accurate position.
        y = (int)m_size.y - y - 1;

        return GetNodeAt(x, y);
    }


    /// <summary>
    /// Get the surrounding nodes in the caridnal directions from given node.
    /// In cases where the node is on the edge of the navmesh, the node will not be included.
    /// </summary>
    /// <param name="node">Node to base check on</param>
    /// <returns>All neighbouring nodes</returns>
    public List<Node> GetNeighbouringNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int x = node.GridPosition.x;
        int y = node.GridPosition.y;

        // Up
        if (y + 1 < m_size.y)
        {
            neighbours.Add(m_nodes[x, y + 1]);
        }

        // Down
        if (y - 1 >= 0)
        {
            neighbours.Add(m_nodes[x, y - 1]);
        }

        // Left
        if (x - 1 >= 0)
        {
            neighbours.Add(m_nodes[x - 1, y]);
        }

        // Right
        if (x + 1 < m_size.x)
        {
            neighbours.Add(m_nodes[x + 1, y]);
        }

        return neighbours;
    }

    #endregion

    #region Private Methods

    private void InternalBake(List<Vector2Int> obstaclePositions, List<Vector2Int> additionalObstacles)
    {
        // First get the size of mesh to initialize node array.
        //Vector2 navmeshSize = CalculateMeshSize();
        Vector2 navmeshSize = m_size;

        // We then initialize the node array with size calculated.
        // To start with, all nodes are walkable.
        m_nodes = new Node[(int)navmeshSize.x + 1, (int)navmeshSize.y];

        
        for (int x = 0; x < navmeshSize.x; x++)
        {
            for (int y = 0; y < navmeshSize.y; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition = new Vector3(x - m_xOffset, 1, y - m_yOffset);

                bool obstructed = !m_groundPositions.Contains(gridPosition);
                m_nodes[x, y] = new Node(gridPosition, worldPosition, obstructed);
            }
        }

        // Finally, we go through all the obstacles and mark the nodes as unwalkable.
        // Since this is comparing world position against grid position, we need to apply offset based on size.
        foreach (Vector2Int obstaclePosition in obstaclePositions)
        {
            Debug.Log(obstaclePosition);
            m_nodes[obstaclePosition.x, obstaclePosition.y].IsObstructed = true;
        }

        Debug.Log("b");

        // Real finally. Include additional obsticles. These should already be respecting the offset
        // so we will set the nodes directly without worrying about the offset.
        foreach (Vector2Int additionalPosition in additionalObstacles)
        {
            m_nodes[additionalPosition.x, additionalPosition.y].IsObstructed = true;
        }

        Debug.Log("c");
    }

    /// <summary>
    /// Determines the size of the mesh based on the ground tiles.
    /// </summary>
    private Vector2 CalculateMeshSize()
    {
        Vector2 size = Vector2.zero;
        m_groundPositions = new HashSet<Vector2Int>();

        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        // Loop through all ground tiles and find the min and max x and y values.
        foreach (Transform child in m_groundHolder)
        {
            if (child.position.x < -minX)
            {
                minX = Mathf.Abs((int)child.position.x);
            }
            else if (child.position.x > maxX)
            {
                maxX = (int)child.position.x;
            }

            // We need to use the Z position as Y, since grid nav mesh will be 2d.
            if (child.position.z < -minY)
            {
                minY = Mathf.Abs((int)child.position.z);
            }
            else if (child.position.z > maxY)
            {
                maxY = (int)child.position.z;
            }          
        }

        // Cahce the minimum values for offset usage later
        m_xOffset = minX;
        m_yOffset = minY;

        // Loop through ground holders again and store positions with offset
        foreach (Transform child in m_groundHolder)
        {
            m_groundPositions.Add(new Vector2Int((int)child.position.x + minX, (int)child.position.z + minY));
        }

        // Now that we have the min and max values, we can calculate the size of the mesh.
        size.x = minX + maxX + 1;
        size.y = minY + maxY + 1;

        // Finally, store the size as well
        m_size = size;

        return size;
    }

    private List<Vector2Int> GetObstaclePositions()
    {
        // First get the size of mesh to initialize node array.
        Vector2 navmeshSize = CalculateMeshSize();
        m_size = navmeshSize;

        List<Vector2Int> obstaclePositions = new List<Vector2Int>();

        // Loop through all obstacle holders and get the positions of all obstacles.
        foreach (Transform obstacleHolder in m_obstacleHolders)
        {
            foreach (Transform obstacle in obstacleHolder)
            {

                int x = (int)obstacle.position.x + m_xOffset;
                int y = (int)obstacle.position.z + m_yOffset;
                obstaclePositions.Add(new Vector2Int(x, y));
            }
        }

        return obstaclePositions;
    }

    #endregion
}
