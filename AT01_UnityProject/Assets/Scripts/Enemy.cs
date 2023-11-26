using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    
    private Node currentNode;
    private Node targetNode;
    private Node lastPlayerNode;
    
    private Player player;
    private Stack<Node> dfsPath = new Stack<Node>();
    private bool playerCaught = false;

    public delegate void GameEndDelegate();
    public event GameEndDelegate GameOverEvent = delegate { };
    
    private void Start()
    {
        InitializeAgent();
        player = GameManager.Instance.Player;
        targetNode = GameManager.Instance.Nodes[0];
    }

    private void Update()
    {
        if (playerCaught)
            return; // game is already over, don't bother

        if (currentNode is null)
        {
            MoveToStartingNode();
        }
        else
        {
            MoveToNextNode();
        }
    }

    // Draw a line showing the intended path
    private void OnDrawGizmos()
    {
        if (currentNode != null)
        {
            Vector3 prevPos = currentNode.tLocation;
        
            foreach (var node in dfsPath)
            {
                var curPos = node.tLocation;
                Debug.DrawLine(prevPos, curPos, Color.magenta);
                prevPos = curPos;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerCaught && other.CompareTag("Player"))
        {
            playerCaught = true;
            GameOverEvent.Invoke(); //invoke the game over event
        }
    }

    private void InitializeAgent()
    {
        currentNode = GameManager.Instance.Nodes[0];
    }

    private void MoveToStartingNode()
    {
        var direction = (targetNode.transform.position - transform.position).normalized;
        transform.Translate(direction * (speed * Time.deltaTime));

        // Reached the node
        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.25f)
        {
            transform.position = targetNode.transform.position;
            currentNode = targetNode;
            
            // Find the new path
            FindPathToPlayer();
        }
    }

    private void MoveToNextNode()
    {
        // If path found
        if (dfsPath.Count > 0)
        {
            targetNode = dfsPath.Peek(); // next node is on top of stack
            var direction = (targetNode.transform.position - transform.position).normalized;
            transform.Translate(direction * (speed * Time.deltaTime));

            // If we have reached target node, pop it from stack
            if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.25f)
            {
                transform.position = targetNode.transform.position;
                currentNode = dfsPath.Pop();
                
                // if the player has moved, recalculate the path
                if (lastPlayerNode != player.CurrentNode)
                {
                    Debug.Log("Player has Moved");
                    FindPathToPlayer();
                }
            }
        }
        // If no path was found or we have reached the destination
        else if (targetNode != null)
        {
            targetNode = null;
            FindPathToPlayer();
        }
    }

    private void FindPathToPlayer()
    {
        lastPlayerNode = player.CurrentNode; // Track the node of the player so if he moves we can update it.
        CalculateDFS(currentNode, player.CurrentNode);
    }


    // Depth-first search algorithm for pathfinding
    private void CalculateDFS(Node startNode, Node endNode)
    {
        var nodeByParentMapping = GetDFSNodeMappings(startNode, endNode);

        if(nodeByParentMapping != null)
        {
            ConstructPath(nodeByParentMapping, endNode); // Construct path if possible
        }
        else
        {
            Debug.Log("No path found in DFS");
        }
    }

    // Construct path based on DFS
    private void ConstructPath(Dictionary<Node, Node> nodeByParentMapping, Node endNode)
    {
        dfsPath.Clear();
        Node curNode = endNode;

        // Go through each parent node
        while (curNode != null)
        {
            dfsPath.Push(curNode);
            curNode = nodeByParentMapping[curNode];
        }
    }

    // Find all node -> parent mappings
    private Dictionary<Node, Node> GetDFSNodeMappings(Node startNode, Node endNode)
    {
        Dictionary<Node, Node> nodeToParentMapping = new Dictionary<Node, Node>();
        HashSet<Node> visitedNodes = new HashSet<Node>();
        Stack<Node> nodesToVisit = new Stack<Node>();

        // Adding start node to mappings and stack
        nodeToParentMapping[startNode] = null;
        nodesToVisit.Push(startNode);
        
        while(nodesToVisit.Count > 0)
        {
            Node curNode = nodesToVisit.Pop();
            visitedNodes.Add(curNode);

            // We found the end node
            if(curNode == endNode)
            {
                Debug.Log("Recalculating path");
                return nodeToParentMapping;
            }
            
            // Going through neighbour nodes
            foreach(Node neighbour in curNode.Neighbours)
            {
                if (visitedNodes.Contains(neighbour))
                {
                    continue;
                }

                // Add to mappings if not tracked previously
                if (!nodeToParentMapping.ContainsKey(neighbour))
                {
                    nodeToParentMapping[neighbour] = curNode;
                }

                // Add to stack for further exploration
                nodesToVisit.Push(neighbour);
            }
        }

        return null;
    }

}