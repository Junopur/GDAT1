using System.Collections.Generic;
using UnityEngine;

public class Enemy: MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private Node currentNode;
    private Node targetNode;
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

        if (currentNode == null)
        {
            MoveToStartingNode();
        }
        else
        {
            MoveToNextNode();
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
        DFS(currentNode, player.CurrentNode);
    }

    private void DFS(Node startNode, Node endNode)
    {
        Dictionary<Node, Node> nodeToParentMapping = new Dictionary<Node, Node>();
        HashSet<Node> visitedNodes = new HashSet<Node>();
        Stack<Node> nodesToVisit = new Stack<Node>();
        
        nodeToParentMapping[startNode] = null;
        nodesToVisit.Push(startNode);
        
        while(nodesToVisit.Count > 0)
        {
            Node currentNode = nodesToVisit.Pop();
            visitedNodes.Add(currentNode);
            
            if(currentNode == endNode)
            {
                Debug.Log("Recalculating path");
                ConstructPath(nodeToParentMapping, endNode);
                return;
            }
            
            foreach(Node neighbour in currentNode.Neighbours)
            {
                if (visitedNodes.Contains(neighbour))
                {
                    continue;
                }

                if (!nodeToParentMapping.ContainsKey(neighbour))
                {
                    nodeToParentMapping[neighbour] = currentNode;
                }
                
                nodesToVisit.Push(neighbour);
            }
        }
        
        // No path found
        Debug.Log("No path found in DFS");
    }

    private void ConstructPath(Dictionary<Node, Node> nodeToParentMapping, Node endNode)
    {
        dfsPath.Clear();

        Node currentNode = endNode;
        
        while(currentNode != null)
        {
            dfsPath.Push(currentNode);
            currentNode = nodeToParentMapping[currentNode];
        }
    }

}