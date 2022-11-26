// /* --------------------
// -----------------------
// Creation date: 03/11/2022
// Author: Alex
// Description: Custom implementation of A* pathfinding algorithm.
// -----------------------
// ------------------- */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    // The navmesh that will be used by pathfinding.
    private NewGridNavMesh m_navmesh;

    /// <summary>
    /// Create a new pathfinder object.
    /// </summary>
    public Pathfinder(NewGridNavMesh navmesh)
    {
        m_navmesh = navmesh;
    }

    /// <summary>
    /// Finds a path from start to end. Path consists of multiple nodes.
    /// Will return null if no path is found, which will need to be handled.
    /// </summary>
    /// <param name="startNode">The node to start searching from</param>
    /// <param name="targetNode">The node to move to</param>
    /// <returns>List of nodes.</returns>
    public List<Node> GetPath(Node startNode, Node targetNode)
    {
        // Create a list of nodes to visit and a list of nodes that have been visited.
        // Here, a hash set is used for performance.
        List<Node> openList = new List<Node>() { startNode };
        HashSet<Node> closedList = new HashSet<Node>();

        // We loop until there are no more unvisited nodes
        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            // Search for a node closer to the target node and make that the current node
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                {
                    currentNode = openList[i];
                }
            }

            // Now the current node should be considered visited.
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // If the current node is the target node, we have found a path.
            if (currentNode == targetNode)
            {
                return GeneratePath(startNode, targetNode);
            }

            // Now we get the neighbouring nodes of the current node to progress.
            foreach (Node neighbourNode in m_navmesh.GetNeighbouringNodes(currentNode))
            {
                // If the neighbor is obstructed or has already been visited, we skip it.
                if (neighbourNode.IsObstructed || closedList.Contains(neighbourNode))
                {
                    continue;
                }

                // Calculate the cost of moving to the neighbour node.
                int moveCost = currentNode.GCost + GetManhattenDistance(currentNode, neighbourNode);

                // If the move cost is less than the neighbour node's current cost, or the neighbour node is not in the open list,
                // we update the neighbour node's cost and parent node.
                if (moveCost < neighbourNode.GCost || !openList.Contains(neighbourNode))
                {
                    neighbourNode.GCost = moveCost;
                    neighbourNode.HCost = GetManhattenDistance(neighbourNode, targetNode);
                    neighbourNode.Parent = currentNode;

                    // If the neighbour node is not in the open list, we add it.
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // If we have finished the while loop and not returned a path, we return null
        // to indicate no path was found.
        return null;
    }

    /// <summary>
    /// Move backwards in order to find the nodes making up the optimal path.
    /// </summary>
    private List<Node> GeneratePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        // We loop until we reach the start node.
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        // Reverse the path so that it starts at the start node.
        path.Reverse();

        return path;
    }

    /// <summary>
    /// Calculates the manhattan distance between two nodes.
    /// It is simply the amount of grids between them, without diagonals.
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Second node</param>
    /// <returns></returns>
    public int GetManhattenDistance(Node a, Node b)
    {
        int x = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
        int y = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
        return x + y;
    }
}
