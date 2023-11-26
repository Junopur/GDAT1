using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Movement speed modifier.")]
    [SerializeField] private float speed = 3;
    private Node currentNode;
    private Node targetNode;
    private Vector3 currentDir;
    private bool playerCaught = false;

    private List<Node> path = new List<Node>();
    
    private static Node PlayerCurrentNode => GameManager.Instance.Player.CurrentNode;
    
    public delegate void GameEndDelegate();
    public event GameEndDelegate GameOverEvent = delegate { };

    // Start is called before the first frame update
    private void Start()
    {
        InitializeAgent();
        targetNode = PlayerCurrentNode;
        FindPath(currentNode, targetNode); //you should replace end node with your destination node.
    }

    // Update is called once per frame
    private void Update()
    {       
        if (playerCaught)
            return;

        if (path.Count > 0 && Vector3.Distance(transform.position, path[0].tLocation) <= 0.25f)
        {
            path.RemoveAt(0);

            if (path.Count > 0)
            {
                currentNode = path[0];
                currentDir = (currentNode.tLocation - transform.position).normalized;
            }
            else
            {
                // Here, the enemy has moved to the next node
                if (targetNode != PlayerCurrentNode) // If the player has moved to another node
                {
                    targetNode = PlayerCurrentNode;
                    Debug.Log($"Player has moved, recalculating and targeting {targetNode.name}");
                    FindPath(currentNode, targetNode); // Recalculate the path
                }
            }
        }

        if (path.Count > 0)
        {
            transform.Translate(currentDir * (speed * Time.deltaTime));
        }
        else if(currentNode != null)
        { 
            Debug.LogWarning($"{name} - No current node");
        }

        var position = transform.position;
        Debug.DrawRay(position, currentDir, Color.cyan);

        if (targetNode != null)
        {
            Debug.DrawLine(position, targetNode.tLocation, Color.red);
        }
    }

    //Called when a collider enters this object's trigger collider.
    //Player or enemy must have rigidbody for this to function correctly.
    private void OnTriggerEnter(Collider other)
    {
        if (playerCaught) 
            return;
        
        if (other.CompareTag("Player"))
        {
            playerCaught = true;
            GameOverEvent.Invoke(); //invoke the game over event
        }
    }

    /// <summary>
    /// Sets the current node to the first in the Game Managers node list.
    /// Sets the current movement direction to the direction of the current node.
    /// </summary>
    private void InitializeAgent()
    {
        currentNode = GameManager.Instance.Nodes[0];
        
        currentDir = currentNode.transform.position - transform.position;
        currentDir = currentDir.normalized;
    }

    //Implement DFS algorithm method here
    private void FindPath(Node start, Node end)
    {
        Dictionary<Node, bool> visited = new Dictionary<Node, bool>();
        Stack<Node> stack = new Stack<Node>(); 
        path = new List<Node>();

        stack.Push(start);

        while (stack.Count > 0)
        {
            Node curNode = stack.Pop();
            if (curNode == end)
            {
                path.Add(curNode);
                return;
            }

            if (visited.ContainsKey(curNode))
            {
                continue;
            }

            visited[curNode] = true;
            path.Add(curNode);

            foreach (Node n in curNode.Children)
            {
                if (!visited.ContainsKey(n))
                {
                    stack.Push(n);
                }
            }
        }
    }
}
