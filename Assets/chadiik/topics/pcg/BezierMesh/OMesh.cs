using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OMesh {

	private Mesh m_Mesh0;

	public Vector3[] vertices;
	public Vector3[] normals;
	public Vector2[] uvs;

	public int[] triangles;

	public int vertIndex;
	public int triIndex;

	public Mesh Mesh {
		get {
			return m_Mesh0;
		}
	}

	public OMesh ( int numVertices = 65535 ) {

		m_Mesh0 = new Mesh ();
		m_Mesh0.MarkDynamic ();

		numVertices = Mathf.Min ( numVertices, 65535 );

		vertices = new Vector3[ numVertices ];
		normals = new Vector3[ numVertices ];
		uvs = new Vector2[ numVertices ];

		int numTriangles = numVertices * 3;
		triangles = new int[ numTriangles ];

	}

	public void Reset () {

		for ( int i = 0, numTriangles = triIndex; i < numTriangles; i++ ) {

			triangles[ i ] = 0;

		}

		vertIndex = 0;
		triIndex = 0;

	}

	public void UpdateBuffers () {

		Mesh mesh = Mesh;
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.UploadMeshData ( false );

	}

	/////

	public class AggregateCache {

		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector2[] uvs;

		public int[] triangles;

		public AggregateCache(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles){

			this.vertices = vertices;
			this.normals = normals;
			this.uvs = uvs;
			this.triangles = triangles;

		}

	}

	private static int c_NextID = 0;
	private static Dictionary<int, AggregateCache> c_Cache = new Dictionary<int, AggregateCache> ();

	public static int NewCacheID () {

		return c_NextID++;

	}

	public static bool Aggregate ( Mesh result, OMesh mesh0, OMesh mesh1, int maxVertices = 65535 ) {

		int vertCount0 = mesh0.vertIndex, triCount0 = mesh0.triIndex;
		int vertCount1 = mesh1.vertIndex, triCount1 = mesh1.triIndex;

		if ( vertCount0 + vertCount1 > maxVertices )
			return false;

		Vector3[] vertices0		= mesh0.vertices;
		Vector3[] normals0		= mesh0.normals;
		Vector2[] uv0			= mesh0.uvs;
		int[]	  triangles0	= mesh0.triangles;

		Vector3[] vertices1		= mesh1.vertices;
		Vector3[] normals1		= mesh1.normals;
		Vector2[] uv1			= mesh1.uvs;
		int[]	  triangles1	= mesh1.triangles;

		MergeBuffers ( result, null,
			vertices0, normals0, uv0, triangles0, vertCount0, triCount0,
			vertices1, normals1, uv1, triangles1, vertCount1, triCount1
		);

		return true;

	}

	public static bool Aggregate ( Mesh result, Mesh mesh0, OMesh mesh1, int cacheID = -1, int maxVertices = 65535 ) {

		AggregateCache cache = null;
		
		if( cacheID != -1 ){

			if ( c_Cache.TryGetValue ( cacheID, out cache ) == false ) {

				cache = new AggregateCache ( mesh0.vertices, mesh0.normals, mesh0.uv, mesh0.triangles );
				c_Cache.Add ( cacheID, cache );
				Debug.Log ( "New AggregateCache created " + cacheID );

			}

		}

		Vector3[] vertices0		= cacheID == -1 ? mesh0.vertices : cache.vertices;
		int[]	  triangles0	= cacheID == -1 ? mesh0.triangles : cache.triangles;

		int vertCount0 = vertices0.Length, triCount0 = triangles0.Length;
		int vertCount1 = mesh1.vertIndex, triCount1 = mesh1.triIndex;

		if ( vertCount0 + vertCount1 > maxVertices )
			return false;

		Vector3[] normals0		= cacheID == -1 ? mesh0.normals : cache.normals;
		Vector2[] uv0			= cacheID == -1 ? mesh0.uv : cache.uvs;

		Vector3[] vertices1		= mesh1.vertices;
		Vector3[] normals1		= mesh1.normals;
		Vector2[] uv1			= mesh1.uvs;
		int[]	  triangles1	= mesh1.triangles;

		MergeBuffers ( result, cache,
			vertices0, normals0, uv0, triangles0, vertCount0, triCount0,
			vertices1, normals1, uv1, triangles1, vertCount1, triCount1
		);

		return true;

	}

	public static void MergeBuffers ( Mesh result, AggregateCache cache,
		Vector3[] vertices0, Vector3[] normals0, Vector2[] uv0, int[] triangles0, int vertCount0, int triCount0, 
		Vector3[] vertices1, Vector3[] normals1, Vector2[] uv1, int[] triangles1, int vertCount1, int triCount1
	) {

		int totalVerts = vertCount0 + vertCount1;
		Vector3[] aVertices = new Vector3[ totalVerts ];
		Vector3[] aNormals = new Vector3[ totalVerts ];
		Vector2[] aUVs = new Vector2[ totalVerts ];

		int totalTris = triCount0 + triCount1;
		int[] aTriangles = new int[ totalTris ];

		//Debug.LogFormat ( "totalVerts = {0} + {1} = {2}, totalTris = {3} + {4} = {5}", vertCount0, vertCount1, totalVerts, triCount0, triCount1, totalTris );

		for ( int i0 = 0; i0 < vertCount0; i0++ ) {

			aVertices[ i0 ]	= vertices0[ i0 ];
			aNormals[ i0 ]	= normals0[ i0 ];
			aUVs[ i0 ]		= uv0[ i0 ];

		}

		for ( int i0 = 0; i0 < triCount0; i0++ ) {

			aTriangles[ i0 ] = triangles0[ i0 ];

		}

		for ( int i1 = 0; i1 < vertCount1; i1++ ) {

			int mIndex = vertCount0 + i1;
			aVertices[ mIndex ]	= vertices1[ i1 ];
			aNormals[ mIndex ]	= normals1[ i1 ];
			aUVs[ mIndex ]		= uv1[ i1 ];

		}

		for ( int i1 = 0; i1 < triCount1; i1++ ) {

			int mIndex = triCount0 + i1;
			aTriangles[ mIndex ] = vertCount0 + triangles1[ i1 ];

		}

		if ( cache != null ) {

			cache.vertices = aVertices;
			cache.normals = aNormals;
			cache.uvs = aUVs;
			cache.triangles = aTriangles;

		}

		result.vertices = aVertices;
		result.normals = aNormals;
		result.uv = aUVs;
		result.triangles = aTriangles;

		result.UploadMeshData ( false );

	}

	public static OMesh From ( Mesh mesh ) {

		Vector3[] vertices		= mesh.vertices;
		Vector3[] normals		= mesh.normals;
		Vector2[] uv			= mesh.uv;
		int[]	  triangles		= mesh.triangles;

		int numVertices = vertices.Length;

		OMesh oMesh = new OMesh ( numVertices );

		oMesh.vertIndex = numVertices;
		oMesh.triIndex = triangles.Length;

		oMesh.vertices = vertices;
		oMesh.normals = normals;
		oMesh.uvs = uv;
		oMesh.triangles = triangles;

		return oMesh;

	}
}
