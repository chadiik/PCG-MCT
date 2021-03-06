﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pattern : MonoBehaviour, IPattern {

	public List<Vector3> vectors;

	public bool needsUpdate = true;

	protected void Awake () {

		needsUpdate = true;

	}

	public virtual void Init () {

		

	}

	public virtual void IterationUpdate ( int index ) {

	}

	public virtual IEnumerator<Vector3> GetEnumerator () {

		if ( needsUpdate ) {

			vectors = new List<Vector3> ();
			Init ();
			needsUpdate = false;

		}

		for ( int i = 0, len = vectors.Count; i < len; i++ ) {

			Vector3 v3 = vectors[i];
			IterationUpdate ( i );
			yield return v3;

		}

	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {

		return GetEnumerator ();

	}

	protected void OnDrawGizmos () {

		foreach (Vector3 v3 in this) {

			Gizmos.DrawWireSphere ( v3, .1f );

		}

	}

	public virtual int Count {
		get {
			return vectors.Count;
		}
	}
}
