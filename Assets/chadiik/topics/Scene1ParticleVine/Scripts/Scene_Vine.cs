using pcg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Vine : MonoBehaviour {

	[System.Serializable]
	public class P {
		public int shapeSubdivisions = 5;
		public float size = 1f;
		public float nodesIntervalDistance = .1f;
	}

	public P p;

	public SplitExtrusion main;
	public Transform mainTarget;

	public void Begin () {

		int shapeDiv = 5;
		List<OShapeExtrusion.Vertex> shape = new List<OShapeExtrusion.Vertex> ( shapeDiv );

		for ( int i = 0; i < shapeDiv; i++ ) {

			float u = ( float )i / ( float )shapeDiv;
			float a = Mathf.PI * 2f * u;
			float size = .2f;

			float c = Mathf.Cos ( a );
			float s = Mathf.Sin ( a );

			Vector2 position = new Vector2 ( c, s ) * size;
			Vector2 normal = ( new Vector2 ( c, s ) * size * 1.5f ).normalized;

			OShapeExtrusion.Vertex vertex = new OShapeExtrusion.Vertex ( position, normal, u );
			shape.Add ( vertex );

		}

		main.ExecuteOnReady ( () => {

			SplitExtrusion.SplineExtruder se = main.New();
			se.SetShape ( shape );

			SplineNode node = new SplineNode ( mainTarget.position, Vector3.zero );
			se.AddNode ( node );

			StartCoroutine ( UpdateCoroutine ( se ) );

		} );

	}

	private IEnumerator UpdateCoroutine ( SplitExtrusion.SplineExtruder se ) {

		yield return new WaitForFixedUpdate ();

		SplineNode lastNode = null;
		for ( int i = 0; i < 1 && mainTarget != null; i++ ) {

			SplineNode node = new SplineNode ( mainTarget.position, Vector3.zero );
			se.AddNode ( node );
			lastNode = node;

			yield return new WaitForFixedUpdate ();

		}

		//se.spline.Smooth ();

		while ( true ) {

			float distanceSquared = Mathf.Max(.01f, p.nodesIntervalDistance * p.nodesIntervalDistance);
			while ( mainTarget != null && ( lastNode.position - mainTarget.position ).sqrMagnitude < distanceSquared )
				yield return new WaitForFixedUpdate ();

			if ( mainTarget == null ) break;

			se.extruder.shapeSize = p.size;

			SplineNode node = new SplineNode ( mainTarget.position, Vector3.zero );
			se.AddNode ( node, 8, .5f );
			lastNode = node;

		}

	}

}
