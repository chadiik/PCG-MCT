using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanOut : MonoBehaviour {

	public float speed = 1f;

	protected void Update () {

		Vector3 direction = transform.TransformPoint ( 0, 0, -1 );
		transform.position = transform.position + direction * speed * Time.deltaTime;

	}

}
