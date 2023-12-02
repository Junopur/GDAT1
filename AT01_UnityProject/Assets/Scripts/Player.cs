using DefaultNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

/*
 * the script will detect input from the keyboard or the input from the on screen buttons 
 * check if the player cube is able to move in chosen direction then move your cube.
 */
public class Player : MonoBehaviour
{
    public Node CurrentNode { get; private set; }
    public Node TargetNode { get; private set; }

    private Vector3 currentDir;

    // Moving Event
    public class OnInputFinalizedEventArgs : EventArgs
    {
        public OnInputFinalizedEventArgs(bool inputValid, MoveDirection direction)
        {
            InputValid = inputValid;
            Direction = direction;
        }
        public MoveDirection Direction { get; set; }
        public bool InputValid { get; set; }
    }
    public event EventHandler<OnInputFinalizedEventArgs> OnInputFinalized;
    
    // Properties
    public bool IsMoving { get; private set; }

    [Header("Input")]
    [SerializeField] private InputManager inputManager;
    
    [Header("Debug")]
    [SerializeField] private float distance = 0.5f;
    [SerializeField] private float speed = 3;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (var node in GameManager.Instance.Nodes)
        {
            if(node.Parents.Length > 2 && node.Children.Length == 0)
            {
                CurrentNode = node;
                break;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Get the input from the Input Manager
        MoveDirection input = inputManager.GetInput();

        // Already Moving
        if (IsMoving)
        {
            // If we are moving, we can't take input
            if (input != MoveDirection.None)
            {
                Debug.Log($"Input while moving: {input} rejected.");
                OnInputFinalized?.Invoke(this, new OnInputFinalizedEventArgs(false, input));
            }
            
            // Move to the next node
            if (Vector3.Distance(transform.position, TargetNode.transform.position) > 0.25f)
            {
                transform.Translate(currentDir * (speed * Time.deltaTime));
            }
            // We have reached the node
            else
            {
                IsMoving = false;
                CurrentNode = TargetNode;
            }
            
            return;
        }
        
        // Not Moving, but Input so we try to move.
        if (input != MoveDirection.None)
        {
            // Is there a node we can move to?
            if (CheckDirection(input, out Node tmpNode))
            {
                // Move to the node
                TargetNode = tmpNode;
                MoveToNode(TargetNode);
                
                // Tell the UI to display input
                OnInputFinalized?.Invoke(this, new OnInputFinalizedEventArgs(true, input)); // Tell the UI it succeeded
            }
            else // There is no node in that direction
            {
                Debug.Log($"No Node in Direction {input}");
                OnInputFinalized?.Invoke(this, new OnInputFinalizedEventArgs(false, input)); // Tell the UI it failed that dir
            }
            
            return;
        }

        // There was no input
        
        /*if (!IsMoving)
        {
            TargetNode = tmpNode;
            
            if (!found)
                return;
            
            if (TargetNode is null)
            {
                Debug.LogError("Target node is null while found is true");
            }

            // Do Movement
            MoveToNode(TargetNode);

        }
        else
        {
            if (Vector3.Distance(transform.position, TargetNode.transform.position) > 0.25f)
            {
                transform.Translate(currentDir * (speed * Time.deltaTime));
            }
            else
            {
                IsMoving = false;
                CurrentNode = TargetNode;
            }
        }*/
    }


    // method for checking if a chosen direction is 'valid'
    //takes in an integer as a parameter.
    //return a variable of type 'node'
    
    
    public bool CheckDirection(MoveDirection nextDir, out Node node)
    {
        node = null;
        
        if (nextDir == MoveDirection.None)
            return false;

        Vector3 direction = nextDir switch
        {
            MoveDirection.Up => Vector3.forward, // north direction positive on the z axis
            MoveDirection.Right => Vector3.right, // north direction positive on the x axis
            MoveDirection.Down => -Vector3.forward, // north direction negative on the z axis
            MoveDirection.Left => -Vector3.right, // north direction negative on the x axis
            _ => Vector3.zero, // unknown
        };

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 100f))
        {
            if (hit.collider.transform.TryGetComponent<Node>(out Node tmpNode))
            {
                node = tmpNode;
                return true;
            }
        }

        Debug.Log("Couldn't find dir");
        return false;
    }

    /// <summary>
    /// Sets the players target node and current directon to the specified node.
    /// </summary>
    /// <param name="node"></param>
    public void MoveToNode(Node node)
    {
        if (IsMoving)
        {
            Debug.LogError("Player is already moving.");
            return;
        }
            
        
        TargetNode = node;
        
        currentDir = (TargetNode.transform.position - transform.position).normalized;

        IsMoving = true;
    }
    

}