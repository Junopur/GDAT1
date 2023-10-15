using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * the script will detect input from the keyboard or the input from the on screen buttons 
 * check if the player cube is able to move in chosen direction then move your cube.
 */
public class MoveCube : MonoBehaviour
{
    //variable of type 'node' called' targetnode'.
    private Node targetNode;
    ///This is the destination we want the player to move towards.
    //variable of type bool for checking if player is 'moving'.
    private bool moving;
    //variable of type float for checking the 'distance' to the 'targetNode'
    [SerializeField] private float distance = 0.5f;
    void update()
    {
        //check if player is moving
        if (!moving)
        {
            //check 4 input
            Vector3 moveDir = Vector3.zero;
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.z = Input.GetAxisRaw("Vertical");

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
        else  // if player is moving then move them towards target node.        //keep checking if player arrives at target node. if they do then switch 'moving' to false.
        {
            if (Vector3.Distance(transform.position, targetNode.transform.position,distance)
            {
                transform.position = Vector3.Lerp(transform.position, targetNode.transform.position,speed, Time.deltaTime);
            }
            else;
            {
                moving = false;

            }
        }
    }

    public void SetDestination(Node node)
    {
        targetNode = node;
        moving = true;
        //update the direction player is facing towards the targetnode.
    }
    //player is not moving

    //the destination of the player
    //takes in variable of type 'node' as a parameter.

    // method for checking if a chosen direction is 'valid'
    //takes in an integer as a parameter.
    //return a variable of type 'node'
    public void CheckDirection(int testDir)
    {
        RaycastHit hit;
        switch (testDir)
        {
            case 0: //north direction positive on the z axis

                if (Physics.Raycast(transform.position, Vector3.forward, out hit, 100f))
                {
                    if (hit.collider.transform.TryGetComponent<Node>(out targetNode node))
                    {
                        //update the destination of the player
                    }
                }
                break;

            case 1: //north direction positve on the x axis

                if (Physics.Raycast(transform.position, Vector3.right, out hit, 100f))
                {
                    if (hit.collider.transform.TryGetComponent<Node>(out targetNode node))
                    {
                        //update the destination of the player
                    }
                }
                break;
            case 2: //north direction negative on the z axis

                if (Physics.Raycast(transform.position, -Vector3.forward, out hit, 100f))
                {
                    if (hit.collider.transform.TryGetComponent<Node>(out targetNode node))
                    {
                        //update the destination of the player
                    }
                }
                break;
            case 3: //north direction negative on the x axis

                if (Physics.Raycast(transform.position, -Vector3.right, out hit, 100f))
                {
                    if (hit.collider.transform.TryGetComponent<Node>(out targetNode node))
                    {
                        //update the destination of the player
                    }
                }
                break;
        }
    }
}
