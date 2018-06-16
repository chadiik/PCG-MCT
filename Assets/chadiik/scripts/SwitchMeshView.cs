using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMeshView : MonoBehaviour {

	
	public static void Apply ( GameObject source, GameObject template, bool meshInstance = true, bool materialInstance = true ) {

		MeshFilter sourceMeshFilter = source.GetComponent<MeshFilter>(),
			meshFilter = template.GetComponent<MeshFilter>();

		if(sourceMeshFilter != null && meshFilter != null ) {

			sourceMeshFilter.sharedMesh = meshInstance ? meshFilter.sharedMesh : Instantiate<Mesh> ( meshFilter.sharedMesh );

		}

		Renderer sourceRenderer = source.GetComponent<Renderer>(),
				renderer = template.GetComponent<Renderer>();

		if ( sourceRenderer != null && renderer != null ) {

			sourceRenderer.sharedMaterial = materialInstance ? renderer.sharedMaterial : Instantiate<Material> ( renderer.sharedMaterial );

		}

	}

}
