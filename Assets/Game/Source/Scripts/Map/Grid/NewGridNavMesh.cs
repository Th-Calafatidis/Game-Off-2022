using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewGridNavMesh
{
    // This is a 2d array representing all the nodes on the navmesh
    private Node[,] m_nodes;

    private Vector2Int m_size;

    // Holders for all objects used to generate the navmesh
    private Transform m_groundHolder;
    private List<Transform> m_obstacleHolders;

    // Offsets used for converting between grid positions and world positions.
    private int m_xOffset;
    private int m_yOffset;

    public int Width
    {
        get { return m_size.x; }
    }

    public int Height
    {
        get { return m_size.y; }
    }

    public NewGridNavMesh(Transform groundHolder, List<Transform> obstacleHolders, bool bakeMesh = false)
    {
        m_groundHolder = groundHolder;
        m_obstacleHolders = obstacleHolders;
        if (bakeMesh) Bake();
    }


    public void Bake()
    {
        List<Vector2Int> ignoredTiles = new List<Vector2Int>();
        List<Vector2Int> additionalBlocked = new List<Vector2Int>();
        InternalBake(ignoredTiles, additionalBlocked);
    }

    public void Bake(List<Vector2Int> extraObstructedTiles)
    {
        List<Vector2Int> ignoredTiles = new List<Vector2Int>();
        InternalBake(ignoredTiles, extraObstructedTiles);
    }

    public void BakeIgnored(List<Vector2Int> ignoredTiles)
    {
        List<Vector2Int> additionalBlocked = new List<Vector2Int>();
        InternalBake(ignoredTiles, additionalBlocked);
    }

    /// <summary>
    /// Returns a node in the given grid position.
    /// Returns null if out of bounds.
    /// </summary>
    /// <param name="position"></param>
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
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Node GetNodeAt(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGridPosition(worldPosition);
        return GetNodeAt(gridPosition.x, gridPosition.y);
    }

    public List<Node> GetNeighbouringNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Get the neighbouring nodes
        Vector2Int[] neighbourPositions = new Vector2Int[]
        {
            new Vector2Int(node.GridPosition.x - 1, node.GridPosition.y),
            new Vector2Int(node.GridPosition.x + 1, node.GridPosition.y),
            new Vector2Int(node.GridPosition.x, node.GridPosition.y - 1),
            new Vector2Int(node.GridPosition.x, node.GridPosition.y + 1),
        };

        // Add the neighbouring nodes to the list
        foreach (Vector2Int neighbourPosition in neighbourPositions)
        {
            Node neighbour = GetNodeAt(neighbourPosition.x, neighbourPosition.y);
            if (neighbour != null)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x) - m_xOffset;
        int y = Mathf.RoundToInt(worldPosition.z) - m_yOffset;

        return new Vector2Int(x, y);
    }

    private void InternalBake(List<Vector2Int> ignoredTiles, List<Vector2Int> additionalObstructed)
    {
        CalculateBounds();
        m_nodes = new Node[m_size.x, m_size.y];

        for (int x = 0; x < m_size.x; x++)
        {
            for (int y = 0; y < m_size.y; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                Vector3 worldPosition = new Vector3(x + m_xOffset, 1, y + m_yOffset);
                m_nodes[x, y] = new Node(gridPosition, worldPosition, true);
            }
        }
      
        // Determine what positions are blocked
        List<Vector2Int> blockedPositions = GetObstructedPositions();
        blockedPositions.AddRange(additionalObstructed);
        blockedPositions.RemoveAll(x => ignoredTiles.Contains(x));

        // Mark tiles as not blocked.
        List<Vector2Int> groundPositions = GetGroundPositions();
        groundPositions.RemoveAll(p => blockedPositions.Contains(p));
        groundPositions.ForEach(p => m_nodes[p.x, p.y].IsObstructed = false);
    }

    /// <summary>
    /// Converts all the positions in the ground holder to a list of Vector2Ints.
    /// </summary>
    /// <returns></returns>
    private List<Vector2Int> GetGroundPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (Transform groundTile in m_groundHolder)
        {
            int x = (int)groundTile.position.x - m_xOffset;
            int y = (int)groundTile.position.z - m_yOffset;
            positions.Add(new Vector2Int(x, y));
        }
        return positions;
    }

    private List<Vector2Int> GetObstructedPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (Transform obstacleHolder in m_obstacleHolders)
        {
            foreach (Transform obstacle in obstacleHolder)
            {
                int x = (int)obstacle.position.x - m_xOffset;
                int y = (int)obstacle.position.z - m_yOffset;
                positions.Add(new Vector2Int(x, y));
            }
        }
        return positions;
    }

    /// <summary>
    /// Uses the ground positions to calculate the width and height of the navmesh.
    /// </summary>
    private void CalculateBounds()
    {
        // From the ground positions, calculate the width and height of the navmesh.
        List<int> checkedX = new List<int>();
        List<int> checkedY = new List<int>();
        foreach (Transform groundTile in m_groundHolder)
        {
            Vector2Int position = new Vector2Int((int)groundTile.position.x, (int)groundTile.position.z);
            if (!checkedX.Contains(position.x))
            {
                checkedX.Add(position.x);
            }

            if (!checkedY.Contains(position.y))
            {
                checkedY.Add(position.y);
            }
        }

        // Set the size of the navmesh.
        m_size = new Vector2Int(checkedX.Count, checkedY.Count);

        // Store the offset as the smallest int from each list
        m_xOffset = checkedX.Min();
        m_yOffset = checkedY.Min();
    }
}
