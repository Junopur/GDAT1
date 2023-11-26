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

    public event EventHandler OnPlayerMovingChanged;

    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            OnPlayerMovingChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public Direction LastMoveDirection { get; set; } = Direction.None;

    [SerializeField] private float distance = 0.5f;
    [SerializeField] private float speed = 3;

    public enum Direction
    {
        None,
        Left,
        Right,
        Up,
        Down,
    }
    
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
        if (!IsMoving)
        {
            //Implement inputs and event-callbacks here
            
            //this overrides the ui input if its detected.
            TakeKeyboardInput();

            bool found = false;
            found = CheckDirection(LastMoveDirection);
            LastMoveDirection = Direction.None;

            if (!found)
                return;
            
            if (TargetNode is null)
            {
                Debug.LogError("Target node is null while found is true");
            }
                
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
        }
    }
    
    //Take axis input and ensure only one is used, this overrides the ui input if its detected.
    private void TakeKeyboardInput()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir != Vector3.zero)
        {
            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.z)) moveDir.z = 0;
            else moveDir.x = 0;

            // pick only one direction to move in right > left > up, down
            if (moveDir == Vector3.right)
                LastMoveDirection = Direction.Right;
            else if (moveDir == Vector3.left)
                LastMoveDirection = Direction.Left;
            else if (moveDir == Vector3.forward)
                LastMoveDirection = Direction.Up;
            else if (moveDir == Vector3.back)
                LastMoveDirection = Direction.Down;
        }
    }

    // method for checking if a chosen direction is 'valid'
    //takes in an integer as a parameter.
    //return a variable of type 'node'
    private bool CheckDirection(Direction nextDir)
    {
        if (nextDir == Direction.None)
            return false;

        Vector3 direction = nextDir switch
        {
            Direction.Up => Vector3.forward, // north direction positive on the z axis
            Direction.Right => Vector3.right, // north direction positive on the x axis
            Direction.Down => -Vector3.forward, // north direction negative on the z axis
            Direction.Left => -Vector3.right, // north direction negative on the x axis
            _ => Vector3.zero, // unknown
        };

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 100f))
        {
            if (hit.collider.transform.TryGetComponent<Node>(out Node tmpNode))
            {
                TargetNode = tmpNode;
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
            return;
        
        TargetNode = node;
        
        currentDir = (TargetNode.transform.position - transform.position).normalized;

        IsMoving = true;
    }
    

}
