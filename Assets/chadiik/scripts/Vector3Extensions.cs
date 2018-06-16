using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions {

	public static bool AlmostEquals(this Vector3 a, Vector3 other ) {

		if ( Mathf.Abs ( other.x - a.x ) > .00001 ) return false;
		if ( Mathf.Abs ( other.y - a.y ) > .00001 ) return false;
		if ( Mathf.Abs ( other.z - a.z ) > .00001 ) return false;

		return true;

	}

	public static bool NearZero ( this Vector3 a ) {

		if ( Mathf.Abs ( a.x ) > .00001 ) return false;
		if ( Mathf.Abs ( a.y ) > .00001 ) return false;
		if ( Mathf.Abs ( a.z ) > .00001 ) return false;

		return true;

	}

}
