using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDSearch : MonoBehaviour {

	public Pattern pattern;
	public ReactionDiffusionMaterial rdTemplate;

	protected void Start () {

		CreateSearchItems ();

	}

	private void Clear () {

		for ( int i = 0; i < transform.childCount; i++ ) {

			Destroy ( transform.GetChild ( i ).gameObject );

		}

	}

	private void CreateSearchItems () {
		int id = 0;

		foreach ( Vector3 point in pattern ) {

			string nameID = id.ToString ();
			id++;

			ReactionDiffusionMaterial rd = Instantiate<ReactionDiffusionMaterial> ( rdTemplate );
			rd.name = nameID;

			rd.transform.position = point;
			rd.transform.parent = transform;

			//rd.Parameters = Instantiate<RDPreset> ( rdTemplate.Parameters );
			rd.texture = Instantiate<CustomRenderTexture> ( rdTemplate.texture );
			rd.texture.name = nameID;
			rd.material = rd.texture.material = Instantiate<Material> ( rdTemplate.material );
			rd.material.name = nameID;

			Renderer renderer = rd.GetComponent<Renderer> ();
			renderer.sharedMaterial = Instantiate<Material> ( rdTemplate.GetComponent<Renderer> ().sharedMaterial );
			renderer.sharedMaterial.name = nameID;
			renderer.sharedMaterial.mainTexture = rd.texture;

			rd.Randomize ();

			StartCoroutine ( AbortRDCoroutine ( rd, 20f ) );

		}
	}

	private IEnumerator AbortRDCoroutine ( ReactionDiffusionMaterial rd, float time ) {

		yield return new WaitForSeconds ( time );

		rd.updateRate = 0;

	}

	protected void Update () {

		if ( Input.GetKeyDown ( KeyCode.Space ) ) {

			Clear ();
			CreateSearchItems ();

		}

	}

}
