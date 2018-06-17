using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infinity : MonoBehaviour {

	public float width, height, speed;
	public float phase = Mathf.PI;

	protected void Start () {

	}

	protected void FixedUpdate () {

		float scale = 1 / ( 3 - Mathf.Cos ( 2 * phase ) );
		Vector3 newPos = new Vector3( width * scale * Mathf.Cos ( phase ), height * scale * Mathf.Sin ( 2 * phase ), 0 );
		phase += speed;

		transform.position = newPos;

	}


	public static Infinity Apply (GameObject target, float width, float height, float speed ) {

		Infinity behaviour = target.AddComponent<Infinity>();

		behaviour.width = width;
		behaviour.height = height;
		behaviour.speed = speed;

		return behaviour;

	}

}
