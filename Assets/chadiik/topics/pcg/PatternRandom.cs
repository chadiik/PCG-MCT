using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PatternRandom : Pattern {

	public enum Configuration { Box, Sphere, SphereSurface };

	public Configuration configuration;
	public int number = 1;
	public float size = 1;
	public Rand rand;
	public Transform center;

	public override void Init () {

		if ( rand == null )
			rand = gameObject.AddComponent<Rand> ();

		if ( center == null )
			center = transform;

		switch ( configuration ) {

			case Configuration.Box:
				InitBox ();
				break;

			case Configuration.Sphere:
				InitSphere ();
				break;

			case Configuration.SphereSurface:
				InitSphereSurface ();
				break;

		}

	}

	private void InitBox () {

		Vector3 centerPos = center.position;
		for (int i = 0; i < number; i++ ) {

			Vector3 v3 = centerPos + (rand.V3() - Vector3.one * .5f) * size;
			vectors.Add ( v3 );

		}

	}

	private void InitSphere () {

		Vector3 centerPos = center.position;
		for ( int i = 0; i < number; i++ ) {

			Vector3 v3 = rand.RangedDirection(Vector3.up, 360) * rand.Float() * size * .5f;
			vectors.Add ( v3 );

		}

	}

	private void InitSphereSurface () {

		Vector3 centerPos = center.position;
		for ( int i = 0; i < number; i++ ) {

			Vector3 v3 = rand.RangedDirection(Vector3.up, 360) * size * .5f;
			vectors.Add ( v3 );

		}

	}
}
