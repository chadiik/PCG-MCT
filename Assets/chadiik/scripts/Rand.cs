using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand : MonoBehaviour {


	public static float Float () {

		float rand = UnityEngine.Random.value;
		return rand;

	}

	public static Vector3 Vector3 () {

		float randX = Float(),
			randY = Float(),
			randZ = Float();

		Vector3 v3 = new Vector3 ( randX, randY, randZ ).normalized;

		return v3;

	}

	public static Vector3 CircleVector3 () {

		float randX = Float () - .5f,
			randY = Float () - .5f,
			randZ = Float () - .5f;

		Vector3 v3 = new Vector3 ( randX, randY, randZ ).normalized;

		return v3;

	}
}
