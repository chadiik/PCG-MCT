using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BackgroundQuad : MonoBehaviour {

	public new Camera camera;
	public float distance = 1f;

	public void Match () {

		Transform camTransform = camera.transform;
		transform.parent = null;

		float h = Mathf.Tan ( camera.fieldOfView * Mathf.Deg2Rad * 0.5f ) * distance * 2f;
		transform.localScale = new Vector3 ( h * camera.aspect, h, 0f );

		transform.position = camTransform.position + camTransform.forward * distance;
		transform.LookAt ( camTransform.position );

	}

	protected void Start () {

		if ( camera == null ) camera = Camera.main;

		Match ();

		transform.parent = camera.transform;

	}

}
