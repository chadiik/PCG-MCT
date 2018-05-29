using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour {

	public Transform target;

	protected void Update () {

		transform.LookAt ( target );

	}
}
