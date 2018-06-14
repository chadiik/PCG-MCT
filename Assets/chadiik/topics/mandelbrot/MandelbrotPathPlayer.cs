using pcg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotPathPlayer : MonoBehaviour {

	public MandlebrotExplorer explorer;
	public MandelbrotPath path;

	public Spline xys, xya;

	public float travelSpeed = 1f;
	public float travelled = 0f;
	public float scale = 0f;

	public Vector3 nomalizer = Vector3.one;

	protected void Start () {

		InitCurve ();

	}

	private float ScaleNormalizer ( float scale ) {

		return 1f;

	}

	private void InitCurve () {

		int numNodes = path.nodes.Count;
		if ( numNodes == 0 ) return;

		int curveResolution = 60;
		xys = new Spline ( curveResolution );

		for ( int i = 0; i < numNodes; i++ ) {

			MandelbrotPath.Node node = path.nodes[ i ];

			float scaleNorm = ScaleNormalizer ( node.s );
			Debug.Log ( scaleNorm );
			Vector3 point = new Vector3 ( node.x * scaleNorm, node.y * scaleNorm, node.s );

			SplineNode splineNode = new SplineNode ( point, Vector3.zero );
			xys.AddNode ( splineNode );

		}

		xys.Smooth ( .25f );
		xys.Update ();

	}

	protected void Update () {

		travelled += Time.deltaTime * travelSpeed;
		travelled = travelled % 1f;

		Vector3 splineNode = xys.GetLocationAlongSpline ( travelled * ( float )xys.nodes.Count );
		int curveIndex = 0;
		float curveTime = 0;
		//Vector3 splineNode = xys.GetLocationAlongSplineAtDistance ( travelled * xys.length, out curveIndex, out curveTime );
		float scaleNorm = ScaleNormalizer ( splineNode.z );
		explorer.x = splineNode.x / scaleNorm;
		explorer.y = splineNode.y / scaleNorm;
		scale = explorer.scale = splineNode.z / nomalizer.z;

		float pathNodeIndex = curveIndex;
		float aTime = curveTime;
		float a1 = path.nodes[ ( int )pathNodeIndex ].a;
		float a2 = path.nodes[ Mathf.Clamp ( ( int )pathNodeIndex + 1, 0, path.nodes.Count ) ].a;
		explorer.rotation = Mathf.Lerp ( a1, a2, aTime );

		/*float s1 = path.nodes[ ( int )pathNodeIndex ].s;
		float s2 = path.nodes[ Mathf.Clamp ( ( int )pathNodeIndex + 1, 0, path.nodes.Count ) ].s;
		explorer.scale = Mathf.Lerp ( s1, s2, aTime );*/

		return;

		MandelbrotPath.Node node = path.nodes[ ( int )( travelled * ( float )( path.nodes.Count - 1 ) ) ];
		explorer.x = node.x;
		explorer.y = node.y;
		scale = explorer.scale = node.s;

	}

}
