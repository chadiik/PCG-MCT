using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public Vector3 rotations;

	protected void Update () {

		transform.rotation = Quaternion.Euler( transform.rotation.eulerAngles + rotations * Time.deltaTime );

	}

}
