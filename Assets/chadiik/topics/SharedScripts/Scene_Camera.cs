using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Camera : MonoBehaviour {

	private static Scene_Camera c_Instance;
	public static Scene_Camera Instance {
		get {
			if ( c_Instance == null )
				c_Instance = GameObject.FindObjectOfType<Scene_Camera> ();

			return c_Instance;
		}
	}


	public new Camera camera;

	private Transform m_LookAtTarget;
	public Transform LookAt {
		set {
			m_LookAtTarget = value;

			if(m_LookAtTarget != null)
				transform.LookAt ( m_LookAtTarget.position );
		}
		get {
			return m_LookAtTarget;
		}
	}

	protected void Start () {
		


	}

	protected void Update () {
		
		if( LookAt != null )
			transform.LookAt ( LookAt.position );

	}

	public void SmoothLookAt( Transform target, float duration, bool assignTarget = true ) {
		StartCoroutine ( SmoothLookAtCoroutine ( target, duration, assignTarget ) );
	}

	private IEnumerator SmoothLookAtCoroutine ( Transform target, float duration, bool assignTarget ) {

		Quaternion startRotation = transform.rotation;
		float startTime = Time.time;
		float t = 0;

		while (t < 1f ) {

			transform.rotation = Quaternion.Lerp ( startRotation, Quaternion.LookRotation ( target.position - transform.position ), t );
			t = ( Time.time - startTime ) / duration;

			yield return new WaitForFixedUpdate ();

		}

		if ( assignTarget )
			LookAt = target;
		else
			transform.rotation = Quaternion.LookRotation ( target.position - transform.position );

	}
}
