using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshFilterExtensions {

	public static Vector3 UVToWorldPos ( this MeshFilter meshFilter, Vector2 uv, int[] indices = null ) {

		Mesh mesh = meshFilter.sharedMesh;
		int[] tris = mesh.triangles;
		Vector2[] uvs = mesh.uv;
		Vector3[] verts = mesh.vertices;

		for ( int i = 0, numTris = indices != null ? indices.Length : tris.Length / 3; i < numTris; i++ ) {

			int triIndex = indices != null ? indices[ i ] * 3 : i / 3;

			Vector2 u1 = uvs[ tris[ triIndex ] ]; // get the triangle UVs
			Vector2 u2 = uvs[ tris[ triIndex + 1 ] ];
			Vector2 u3 = uvs[ tris[ triIndex + 2 ] ];

			// calculate triangle area - if zero, skip it
			float a = SignedTriArea ( u1, u2, u3 );
			if ( a == 0 ) continue;

			// calculate barycentric coordinates of u1, u2 and u3
			// if anyone is negative, point is outside the triangle: skip it
			float a1 = SignedTriArea ( u2, u3, uv ) / a;
			if ( a1 < 0 ) continue;
			float a2 = SignedTriArea ( u3, u1, uv ) / a;
			if ( a2 < 0 ) continue;
			float a3 = SignedTriArea ( u1, u2, uv ) / a;
			if ( a3 < 0 ) continue;

			// point inside the triangle - find mesh position by interpolation...
			Vector3 p3D = a1 * verts[ tris[ triIndex ] ] + a2 * verts[ tris[ triIndex + 1 ] ] + a3 * verts[ tris[ triIndex + 2 ] ];

			// and return it in world coordinates:
			return meshFilter.transform.TransformPoint ( p3D );

		}

		// point outside any uv triangle: return Vector3.zero
		return Vector3.zero;

	}

	// calculate signed triangle area using a kind of "2D cross product":
	public static float SignedTriArea ( Vector2 p1, Vector2 p2, Vector2 p3 ) {

		Vector2 v1 = p1 - p3;
		Vector2 v2 = p2 - p3;

		return ( v1.x * v2.y - v1.y * v2.x ) / 2;

	}

}
