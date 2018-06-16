using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace pcg {
	public class OShapeExtrusion {

		private OMesh m_OMesh;
		private Spline m_Spline;
		private bool m_NeedsUpdate = true;

		// Operating values
		private List<OrientedPoint> m_SplinePath = new List<OrientedPoint> ();
		private int m_SplinePathIndex = 0;
		private float m_SplineT = 0;
		private int m_MeshingNodeIndex = 0;
		private int m_MeshingIndex = 0;
		
		private int m_MaxVertices;


		public float shapeSize = 1;
		public float textureScale = 1;
		public List<Vertex> shapeVertices = new List<Vertex> ();

		public UnityEvent Overflow = new UnityEvent ();

		public Mesh Mesh {
			get {
				return m_OMesh.Mesh;
			}
		}

		public OMesh OMesh {
			get {
				return m_OMesh;
			}
		}

		public Spline Spline {
			get {
				return m_Spline;
			}

			set {
				m_Spline = value;
				m_Spline.CurveChanged.AddListener ( () => m_NeedsUpdate = true );
				m_NeedsUpdate = true;
			}
		}

		public bool NeedsUpdate {
			get {
				return m_NeedsUpdate;
			}
		}

		public OShapeExtrusion ( int maxVertices = 65535 ) {

			m_OMesh = new OMesh ( maxVertices );
			this.m_MaxVertices = maxVertices;

		}

		public void Reset () {

			m_SplinePath.Clear ();

			m_MeshingNodeIndex = 0;
			m_MeshingIndex = 0;

			m_SplinePathIndex = 0;
			m_SplineT = 0;

			m_OMesh.Reset ();

			m_NeedsUpdate = true;

		}

		public int Renew () {

			int nodeIndex = Spline.GetNodeIndexForTime ( m_SplineT );

			m_SplinePath.Clear ();

			m_MeshingNodeIndex = 0;
			m_MeshingIndex = 0;

			m_SplinePathIndex = 0;
			m_SplineT = 0;

			m_OMesh = new OMesh ( m_MaxVertices );

			m_NeedsUpdate = true;

			return nodeIndex;

		}

		public void Update ( int keepBuffer = 0 ) {

			if ( NeedsUpdate ) {

				m_NeedsUpdate = false;

				GenerateMesh ( keepBuffer );

			}

		}

		private void GenerateMesh ( int keepBuffer ) {

			UpdatePath ( keepBuffer );
			if ( m_SplinePathIndex < 2 ) return;

			int numPath = m_SplinePath.Count;
			int vertsInShape = shapeVertices.Count;

			int[] triangleIndices = m_OMesh.triangles;
			Vector3[] vertices = m_OMesh.vertices;
			Vector3[] normals = m_OMesh.normals;
			Vector2[] uvs = m_OMesh.uvs;

			int segments = numPath - m_MeshingNodeIndex;

			int tLengthNeeded = (m_MeshingIndex + segments * vertsInShape) * 6;
			
			if ( tLengthNeeded >= triangleIndices.Length ) {

				if ( Overflow != null ) Overflow.Invoke ();

				return;

			}

			int updateOffset = m_MeshingIndex;

			for ( int i = 0; i < segments; i++ ) {

				OrientedPoint op = m_SplinePath[ m_MeshingNodeIndex ];
				float vCoord = ( float )i / ( ( float )segments ) * textureScale;

				for ( int iVertex = 0; iVertex < vertsInShape; iVertex++ ) {

					Vertex v = shapeVertices[ iVertex ];

					vertices[ m_MeshingIndex ] = op.LocalToWorld ( v.point * shapeSize );
					normals[ m_MeshingIndex ] = op.LocalToWorldDirection ( v.normal );
					uvs[ m_MeshingIndex ] = new Vector2 ( v.uCoord, vCoord );
					m_MeshingIndex++;

				}

				m_MeshingNodeIndex++;

			}

			int index = updateOffset;
			int ti = updateOffset * 6;

			if ( index > vertsInShape ) index -= vertsInShape;
			else segments--;

			for ( int i = 0; i < segments; i++ ) {

				for ( int iVertex = 0; iVertex < vertsInShape; iVertex++ ) {

					int offset = 
						( iVertex == ( vertsInShape - 1 ) ) ?
						-( vertsInShape - 1 )
						: 1;
					int a = index + vertsInShape;
					int b = index;
					int c = index + offset;
					int d = index + offset + vertsInShape;
					
					triangleIndices[ ti++ ] = c;
					triangleIndices[ ti++ ] = a;
					triangleIndices[ ti++ ] = b;
					triangleIndices[ ti++ ] = a;
					triangleIndices[ ti++ ] = c;
					triangleIndices[ ti++ ] = d;

					index++;

				}

			}

			m_OMesh.vertIndex = m_MeshingIndex;
			m_OMesh.triIndex = ti;
			m_OMesh.UpdateBuffers ();

		}

		private void UpdatePath ( int keepBuffer ) {

			int numNodes = m_Spline.nodes.Count - keepBuffer;
			if ( numNodes < 2 ) 
				return;

			if ( m_SplinePath.Count > m_SplinePathIndex ) {

				m_SplinePath.RemoveRange ( m_SplinePathIndex, m_SplinePath.Count - m_SplinePathIndex );

			}

			Vector3 tangent = m_Spline.GetTangentAlongSpline ( m_SplineT - .25f );
			Quaternion rotation = CubicBezierCurve.GetRotationFromTangent ( tangent, Vector3.forward );

			float step = 1f / (float)(m_Spline.CurveResolution + 1);

			for ( float to = numNodes - 1; m_SplineT < to; m_SplineT += step ) {

				Vector3 point = m_Spline.GetLocationAlongSpline ( m_SplineT );
				tangent = m_Spline.GetTangentAlongSpline ( m_SplineT );
				Quaternion tRot = CubicBezierCurve.GetRotationFromTangent ( tangent, Vector3.forward );
				rotation = tRot;// Quaternion.Lerp ( rotation, tRot, .5f );

				m_SplinePath.Add ( new OrientedPoint ( point, rotation ) );
				m_SplinePathIndex++;

			}

		}

		public override string ToString () {

			string str = "OShapeExtrusion(ni:" + m_MeshingNodeIndex + ", mi:" + m_MeshingIndex + ")";
			return str;

		}



		[Serializable]
		public class Vertex {

			public Vector2 point;
			public Vector2 normal;
			public float uCoord;

			public Vertex ( Vector2 point, Vector2 normal, float uCoord ) {

				this.point = point;
				this.normal = normal;
				this.uCoord = uCoord;

			}

		}

		public struct OrientedPoint {

			public Vector3 position;
			public Quaternion rotation;

			public OrientedPoint ( Vector3 position, Quaternion rotation ) {

				this.position = position;
				this.rotation = rotation;

			}

			public Vector3 LocalToWorld ( Vector3 point ) {

				return position + rotation * point;

			}

			public Vector3 LocalToWorldDirection ( Vector3 dir ) {

				return rotation * dir;

			}
		}
	}
}