using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotPath : ScriptableObject {

	[System.Serializable]
	public struct Node {

		public float x, y, s, a;

	}

	public List<Node> nodes = new List<Node>();


	public void AddNode ( float x, float y, float s, float a ) {

		Node node = new Node () { x = x, y = y, s = s, a = a };

		AddNode ( node );

	}

	public void AddNode ( Node node ) {

		nodes.Add ( node );

	}

	public void RemoveLastNode () {

		nodes.RemoveAt ( nodes.Count - 1 );

	}
}
