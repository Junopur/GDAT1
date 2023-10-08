using System.Collections;
using System.Collections.Generic;
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
    public Node[] Children { get { return children; } }
    /// <summary>
    /// Returns the parents of the node.
    /// </summary>
    public Node[] Parents { get { return parents; } }

    private Vector3 offset = new Vector3(0, 1, 0);
}


