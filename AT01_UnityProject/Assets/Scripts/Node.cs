using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Tooltip("Parent directly above node should always be first in array.")]
    [SerializeField] private Node[] parents;
    [Tooltip("Child directly below node should always be first in array.")]
    [SerializeField] private Node[] children;

    /// <summary>
    /// Returns the children of the node.
    /// </summary>
    [SerializeField] public List<Node> listChildren = new List<Node>();
    public Node[] Children => children;

    /// <summary>
    /// Returns the parents of the node.
    /// </summary>
    public Node[] Parents => parents;

    public Node[] Neighbours => Parents.Concat(Children).ToArray();
    
    public Vector3 tLocation;

    private Vector3 offset = new Vector3(0, 1, 0);
    private void Awake()
    {
        tLocation = transform.position;

        foreach (Node node in children)
        {
            listChildren.Add(node);
        }
    }
    private void OnDrawGizmos()
    {
        //Draws red lines between a parent and its children.
        if (parents.Length > 0)
        {
            foreach (Node node in parents)
            {
                Debug.DrawLine(transform.position, node.transform.position, Color.red);
            }
        }
        //Draws green lines between a child and its children.
        if (children.Length > 0)
        {
            foreach (Node node in children)
            {
                Debug.DrawLine(transform.position + offset, node.transform.position + offset, Color.green);
            }
        }
    }
}
