using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {
	
	public Tile parentNode = null;
	public float gScore = Mathf.Infinity;
	public float fScore = Mathf.Infinity;

}
