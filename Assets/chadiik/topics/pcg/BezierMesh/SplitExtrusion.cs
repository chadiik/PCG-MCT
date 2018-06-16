using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace pcg {
	public class SplitExtrusion : MonoBehaviour {

		public class SplineExtruder {

			private Transform m_View;
			private List<Transform> m_Views = new List<Transform>();

			public OShapeExtrusion extruder;
			public Spline spline;

			public const int MAX_VERTICES = 16384;

			public Transform View {
				set {

					m_View = value;
					m_Views.Add ( m_View );
					SplitExtrusion thisBehaviour = m_View.GetComponent<SplitExtrusion> ();
					
					if ( thisBehaviour != null )
						thisBehaviour.DeleteImmediate ();

					MeshFilter meshFilter = m_View.GetComponent<MeshFilter> ();
					meshFilter.sharedMesh = extruder.Mesh;

				}

				get {

					return m_View;

				}
			}

			public List<Transform> Views {
				get {
					return m_Views;
				}
			}

			public SplineExtruder ( int curveResolution, Transform template, Transform parent ) {

				extruder = new OShapeExtrusion ( MAX_VERTICES );
				spline = new Spline ( curveResolution );
				extruder.Spline = spline;

				View = Instantiate<Transform> ( template, parent );

				extruder.Overflow.AddListener ( () => {

					int nodeIndex = extruder.Renew ();
					nodeIndex = Mathf.Max ( nodeIndex - 2, 0);
					spline = spline.ExtractNew ( nodeIndex );
					Debug.Log ( spline );
					extruder.Spline = spline;

					View = Instantiate<Transform> ( template, parent );

				} );

			}

			private void Renew () {

				

			}

			public void AddNode ( SplineNode node, int smoothLength = 0, float smooth = 0f ) {

				int numNodes = spline.AddNode ( node );

				if ( smoothLength > 0 )
					spline.Smooth ( smooth, smoothLength );

				if ( numNodes > 1 ) {

					spline.Update ();
					extruder.Update ( 4 );

					extruder.Mesh.RecalculateBounds ();

				}

			}

			public void SetShape ( List<OShapeExtrusion.Vertex> shape ) {

				extruder.shapeVertices = shape;

			}

			public void Reset () {

				spline.Reset ();
				extruder.Reset ();

				extruder.Mesh.Clear ();

			}

		}

		private Mesh m_AggregateMesh;
		private SimplePool<SplineExtruder> m_SplineExtruders;
		private SplineExtruder m_CurrentSplineExtruder;

		public int curveResolution = 2;
		public Transform template;

		public SplineExtruder CurrentSplineExtruder {
			get {

				return m_CurrentSplineExtruder;

			}
		}

		public UnityEvent OnReady;

		/////

		protected void Start () {

			Constructor ();

		}

		private void Constructor () {

			m_SplineExtruders = new SimplePool<SplitExtrusion.SplineExtruder> ();

			CreateAggregateMesh ();

			if ( OnReady != null ) {

				OnReady.Invoke ();

			}

		}

		public void ExecuteOnReady ( UnityAction action ) {

			OnReady.AddListener ( action );

			if ( m_AggregateMesh != null ) {

				action.Invoke ();

			}

		}

		private void CreateAggregateMesh () {

			Transform view = Instantiate<Transform> ( template, transform );
			MeshFilter meshFilter = view.GetComponent<MeshFilter> ();
			m_AggregateMesh = meshFilter.sharedMesh;

			if ( m_AggregateMesh == null ) {

				m_AggregateMesh = new Mesh ();
				meshFilter.sharedMesh = m_AggregateMesh;

			}

			m_AggregateMesh.MarkDynamic ();

			m_OMeshCacheID = OMesh.NewCacheID ();

		}

		private SplineExtruder RequestSplineExtruder() {

			SplineExtruder se = m_SplineExtruders.Get ();

			if ( se == null ) {

				se = new SplineExtruder ( curveResolution, template, transform );

				m_SplineExtruders.Add ( se );

				se = m_SplineExtruders.Get ();

			}

			m_CurrentSplineExtruder = se;

			return se;

		}

		public SplineExtruder New () {

			SplineExtruder se = RequestSplineExtruder ();

			return se;

		}

		private int m_OMeshCacheID;
		public bool cachedOMesh = true;

		private void Aggregate ( SplineExtruder se ) {

			bool aggregate = OMesh.Aggregate ( m_AggregateMesh, m_AggregateMesh, se.extruder.OMesh, cachedOMesh ? m_OMeshCacheID : -1 );

			if ( aggregate == false ) {

				CreateAggregateMesh ();
				OMesh.Aggregate ( m_AggregateMesh, m_AggregateMesh, se.extruder.OMesh, cachedOMesh ? m_OMeshCacheID : -1 );

			}

			m_AggregateMesh.RecalculateBounds ();

		}

		public void End ( SplineExtruder se ) {

			Aggregate ( se );
			se.Reset ();

			m_SplineExtruders.Return ( se );

		}

		public void DeleteImmediate () {

			DestroyImmediate ( this );

		}

	}
}