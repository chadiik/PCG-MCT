using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleAffector : MonoBehaviour {

	public float influenceStartRadius = 0;
	public float influenceSpan = 1;
	public float strength = 1;
	public AnimationCurve influence;

	protected void Start () {

		float s = strength > 0 ? ( influenceStartRadius + influenceSpan ) * 2f : ( influenceStartRadius + influenceSpan );
		transform.localScale = new Vector3(s, s, s);

	}

}
