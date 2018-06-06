using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabeledPoint : MonoBehaviour {

	private Transform m_LP;
	
	public Transform box;
	public TextMesh textMesh;

	public Transform Point {

		get {

			if ( m_LP == null ) {

				m_LP = (( GameObject )GameObject.Instantiate ( Resources.Load ( "LabeledPoint" ) )).transform;

				textMesh = m_LP.GetComponentInChildren<TextMesh> ();
				Transform[] children = m_LP.GetComponentsInChildren<Transform> ();

				box = children[ 0 ] == textMesh.transform ? children[ 1 ] : children[ 0 ];

				m_LP.parent = transform;
				m_LP.localPosition = Vector3.zero;

			}

			return m_LP;

		}

	}

	protected void Awake () {

		Transform point = Point;

	}
	
}
