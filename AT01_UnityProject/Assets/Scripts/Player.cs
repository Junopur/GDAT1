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
    
    private bool isMoving;
    
    [SerializeField] private float distance = 0.5f;
    [SerializeField] private float speed = 3;

    public enum Direction
    {
        NorthPosZ,
        NorthPosX,
        NorthNegZ,
        NorthNegX,
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
        if (!isMoving)
        {
            //Implement inputs and event-callbacks here
            Vector3 moveDir = Vector3.zero;
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.z = Input.GetAxisRaw("Vertical");
            
            Debug.Log("player moveDir: " + moveDir);

            bool found = false;
            if (moveDir.x < 0)
            {
                //call the check direction and pass it 3
                found = CheckDirection(3);
            }
            else if (moveDir.x > 0)
            {
                //call the check direction and pass it 1
                found = CheckDirection(1);
            }
            else if (moveDir.z < 0)
            {
                //call the check direction and pass it 2
                found = CheckDirection(2);
            }
            else if (moveDir.z > 0)
            {
                //call the check direction and pass it 0
                found = CheckDirection(0);
            }

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
                isMoving = false;
                CurrentNode = TargetNode;
            }
        }
    }

    //Implement mouse interaction method here

    /// <summary>
    /// Sets the players target node and current directon to the specified node.
    /// </summary>
    /// <param name="node"></param>
    public void MoveToNode(Node node)
    {
        if (isMoving)
            return;
        
        TargetNode = node;
        
        currentDir = (TargetNode.transform.position - transform.position).normalized;

        isMoving = true;
    }
    
    // method for checking if a chosen direction is 'valid'
    //takes in an integer as a parameter.
    //return a variable of type 'node'
    private bool CheckDirection(int intDir)
    {
        Vector3 direction = intDir switch
        {
            0 => Vector3.forward, // north direction positive on the z axis
            1 => Vector3.right, // north direction positive on the x axis
            2 => -Vector3.forward, // north direction negative on the z axis
            3 => -Vector3.right, // north direction negative on the x axis
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
    
    /*private void Update()
    {
        //check if player is moving
        Debug.Log("Play is moving: " + isMoving);
        
        if (!isMoving)
        {
            //check 4 input
            var moveDir = Vector3.zero;
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.z = Input.GetAxisRaw("Vertical");
            
            Debug.Log("player moveDir: " + moveDir);

            if (moveDir.x < 0)
            {
                //call the check direction and pass it 3
                CheckDirection(3);
            }
            else if (moveDir.x > 0)
            {
                //call the check direction and pass it 1
                CheckDirection(1);
            }
            else if (moveDir.z < 0)
            {
                //call the check direction and pass it 2
                CheckDirection(2);
            }
            else if (moveDir.z > 0)
            {
                //call the check direction and pass it 0
                CheckDirection(0);
            }


        }
        else // if player is moving then move them towards target node. keep checking if player arrives at target node. if they do then switch 'moving' to false.
        {
            if (Vector3.Distance(transform.position, targetNode.transform.position) < distance)
            {
                transform.position = Vector3.Lerp(transform.position, targetNode.transform.position, speed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
            }
        }
    }

    //player is not moving
    //the destination of the player
    //takes in variable of type 'node' as a parameter.
    public void SetDestination(Node node)
    {
        targetNode = node;
        isMoving = true;
        
        //update the direction player is facing towards the targetnode.
        
    } 
    
*/
}
