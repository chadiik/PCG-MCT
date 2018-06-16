using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;


namespace pcg {

	[Serializable]
	public class Spline {

		protected int m_Resolution;
		protected int m_CurveResolution;
		protected bool m_NeedsUpdate = true;

		public int CurveResolution {
			get {
				return m_CurveResolution;
			}
		}

		public List<SplineNode> nodes;

		public List<CubicBezierCurve> curves;

		/// <summary>
		/// The spline length in world units.
		/// </summary>
		public float length;

		/// <summary>
		/// Event raised when one of the curve changes.
		/// </summary>
		[HideInInspector]
		public UnityEvent CurveChanged = new UnityEvent ();

		public bool HasEnoughNodes {
			get {

				return nodes.Count >= 2;

			}
		}

		public Spline (int curveResolution = 2) {

			m_Resolution = curveResolution;
			m_CurveResolution = curveResolution;

			if(nodes == null) nodes = new List<SplineNode> ();
			if(curves == null) curves = new List<CubicBezierCurve> ();

		}

		public void Reset () {

			nodes.Clear ();
			curves.Clear ();
			length = 0;

			m_NeedsUpdate = true;

		}

		public Spline ExtractNew ( int keepFrom ) {

			Spline spline = new Spline ( CurveResolution );

			Debug.Log ( keepFrom + "/" + nodes.Count );
			for ( int i = keepFrom, len = nodes.Count; i < len; i++ ) {

				SplineNode node = nodes[ i ];
				spline.AddNode ( node );

			}

			return spline;

		}

		public void Smooth ( float t = .5f, int length = 0 ) {

			int numNodes = nodes.Count;
			if ( numNodes < 4 ) return;

			int offset = Mathf.Max(0, numNodes - length);

			int numDeltas = numNodes - 1;
			Vector3[] deltas = new Vector3[ numDeltas ];

			Vector3 location = nodes[ 0 ].position;

			for ( int i = 1 + offset; i < numNodes; i++ ) {

				Vector3 currentLocation = nodes[ i ].position;
				deltas[ i - 1 ] = currentLocation - location;
				location = currentLocation;

			}

			for ( int i = 0 + offset; i < numDeltas - 1; i++ ) {

				SplineNode node = nodes[ i + 1 ];
				Vector3 overshoot = deltas[ i ] * t;
				Vector3 towardsNext = deltas[ i + 1 ] * t;
				node.SetDirection ( node.position + ( overshoot + towardsNext ) * t );

			}

			Vector3 midToThird, midToSecond;

			// Smooth first
			if ( offset < 4 ) {
				SplineNode first = nodes[ 0 ];
				SplineNode second = nodes[ 1 ];
				SplineNode third = nodes[ 2 ];

				midToThird = ( third.position + first.position ) * t;
				midToSecond = ( midToThird + second.position ) * t;
				first.SetDirection ( first.position + midToSecond - midToThird );
			}

			// Smooth last
			SplineNode last = nodes[ numNodes - 1 ];
			SplineNode secondToLast = nodes[ numNodes - 2 ];
			SplineNode thirdToLast = nodes[ numNodes - 3 ];

			midToThird = ( thirdToLast.position + last.position ) * t;
			midToSecond = ( midToThird + secondToLast.position ) * t;
			last.SetDirection ( last.position - ( midToSecond - midToThird ) );

			m_NeedsUpdate = true;

		}

		private void CreateCurves () {

			curves.Clear ();

			for ( int i = 0, len = nodes.Count - 1; i < len; i++ ) {

				SplineNode n = nodes[ i ];
				SplineNode next = nodes[ i + 1 ];

				CubicBezierCurve curve = new CubicBezierCurve ( n, next, m_CurveResolution );
				curves.Add ( curve );

			}

			m_NeedsUpdate = true;

		}

		public bool NeedsUpdate {
			get {
				bool needsUpdate = m_NeedsUpdate;
				for ( int i = 0, numCurves = curves.Count; i < numCurves; i++ ) needsUpdate = needsUpdate || curves[ i ].NeedsUpdate;
				return needsUpdate;
			}
		}

		public void Update () {

			if ( NeedsUpdate ) {

				m_NeedsUpdate = false;

				length = 0;
				for ( int i = 0, numCurves = curves.Count; i < numCurves; i++ ) {

					CubicBezierCurve curve = curves[ i ];
					curve.Update ();
					length += curve.Length;

				}

				if ( CurveChanged != null ) {

					CurveChanged.Invoke ();

				}
			}

		}

		/// <summary>
		/// Returns the point on spline at time. Time must be between 0 and the nodes count.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Vector3 GetLocationAlongSpline ( float t ) {

			int curveIndex = GetCurveIndexForTime ( t );
			return curves[ curveIndex ].GetLocation ( t - curveIndex );

		}

		/// <summary>
		/// Returns the tangent of spline at time. Time must be between 0 and the nodes count.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Vector3 GetTangentAlongSpline ( float t ) {

			int curveIndex = GetCurveIndexForTime ( t );
			return curves[ curveIndex ].GetTangent ( t - curveIndex );

		}

		public virtual int GetCurveIndexForTime ( float t ) {

			return GetNodeIndexForTime ( t );

		}

		public int GetNodeIndexForTime ( float t ) {

			t = Mathf.Clamp ( t, 0, (float)nodes.Count - 1 );
			
			int index = Mathf.FloorToInt ( t );

			if ( index == nodes.Count - 1 )
				index--;

			return index;

		}

		/// <summary>
		/// Returns the point on spline at distance. Distance must be between 0 and spline length.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public Vector3 GetLocationAlongSplineAtDistance ( float d, out int curveIndex, out float t ) {

			d = Mathf.Clamp ( d, 0, length );

			//Debug.LogFormat ( "{0}/{1}", d.ToString ( ".00" ), length.ToString ( ".00" ) );
			
			for(int i = 0, numCurves = curves.Count; i < numCurves; i++){

				CubicBezierCurve curve = curves[i];

				//Debug.LogFormat ( "cd({0}) = {1}", i, curve.Length.ToString ( ".00" ) );

				if ( d > curve.Length ) {

					d -= curve.Length;

				}
				else {

					curveIndex = i;
					t = d / curve.Length;
					return curve.GetLocationAtDistance ( d );

				}
			}

			throw new Exception ( "Something went wrong with GetLocationAlongSplineAtDistance" );

		}

		/// <summary>
		/// Returns the tangent of spline at distance. Distance must be between 0 and spline length.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public Vector3 GetTangentAlongSplineAtDistance ( float d ) {

			d = Mathf.Clamp ( d, 0, length );

			for ( int i = 0, numCurves = curves.Count; i < numCurves; i++ ) {

				CubicBezierCurve curve = curves[ i ];

				if ( d > curve.Length ) {

					d -= curve.Length;

				}
				else {

					return curve.GetTangentAtDistance ( d );

				}
			}

			throw new Exception ( "Something went wrong with GetTangentAlongSplineAtDistance" );

		}

		/// <summary>
		/// Adds a node at the end of the spline.
		/// </summary>
		/// <param name="node"></param>
		public virtual int AddNode ( SplineNode node ) {

			nodes.Add ( node );

			int numNodes = nodes.Count;
			if ( numNodes != 1 ) {

				SplineNode previousNode = nodes[ numNodes - 2 ];
				CubicBezierCurve curve = new CubicBezierCurve ( previousNode, node, m_CurveResolution );
				curves.Add ( curve );

			}

			m_NeedsUpdate = true;

			return numNodes;

		}

		/// <summary>
		/// Insert the given node in the spline at index. Index must be greater than 0 and less than node count.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="node"></param>
		public void InsertNode ( int index, SplineNode node ) {

			if ( index == 0 )
				throw new Exception ( "Can't insert a node at index 0" );

			SplineNode previousNode = nodes[ index - 1 ];
			SplineNode nextNode = nodes[ index ];

			nodes.Insert ( index, node );

			curves[ index - 1 ].ConnectEnd ( node );

			CubicBezierCurve curve = new CubicBezierCurve ( node, nextNode, m_CurveResolution );
			curves.Insert ( index, curve );

			m_NeedsUpdate = true;

		}

		/// <summary>
		/// Remove the given node from the spline. The given node must exist and the spline must have more than 2 nodes.
		/// </summary>
		/// <param name="node"></param>
		public void RemoveNode ( SplineNode node ) {

			int index = nodes.IndexOf ( node );

			if ( nodes.Count <= 2 ) {

				throw new Exception ( "Can't remove the node because a spline needs at least 2 nodes." );

			}

			CubicBezierCurve toRemove = index == nodes.Count - 1 ? curves[ index - 1 ] : curves[ index ];

			if ( index != 0 && index != nodes.Count - 1 ) {

				SplineNode nextNode = nodes[ index + 1 ];
				curves[ index - 1 ].ConnectEnd ( nextNode );

			}

			nodes.RemoveAt ( index );
			curves.Remove ( toRemove );

			m_NeedsUpdate = true;

		}


		private int m_IterCurveIndex = 0;
		public CubicBezierCurve.CurveSample NewIteration () {

			m_IterCurveIndex = 0;
			CubicBezierCurve.CurveSample sample = curves[ 0 ].NewIteration ();

			return sample;

		}

		public CubicBezierCurve.CurveSample Iterate () {

			if(m_IterCurveIndex >= curves.Count)
				return null;

			CubicBezierCurve curve = curves[m_IterCurveIndex];
			CubicBezierCurve.CurveSample sample = curve.Iterate ();

			if ( sample == null ) {

				m_IterCurveIndex++;
				
				if ( m_IterCurveIndex < curves.Count ) {

					sample = curves[ m_IterCurveIndex ].NewIteration ();

				}

			}

			return sample;

		}

		public override string ToString () {

			string str = "Spline(" + nodes.Count + ")";
			return str;

		}
	}
}