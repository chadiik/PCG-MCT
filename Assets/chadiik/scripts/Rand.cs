using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rand : MonoBehaviour {

	private static Rand m_DefaultInstance;

	public static Rand Instance {

		get {

			if ( m_DefaultInstance == null ) {

				m_DefaultInstance = GameObject.FindObjectOfType<Rand> ();

				if ( m_DefaultInstance == null ) {

					GameObject randObject = new GameObject ( "Random Number Generator" );
					m_DefaultInstance = randObject.AddComponent<Rand> ();

				}

			}

			return m_DefaultInstance;

		}

	}

	public System.Random generator;
	public int seed = 0;

	private int m_Seeded = 0;

	protected void Awake () {

		if ( m_Seeded == 0 )
			Seed ( seed );

	}

	public void Seed ( int seed ) {

		Debug.Log ( "New seed: " + seed );
		this.seed = seed;
		generator = new System.Random ( this.seed );
		m_Seeded++;

	}

	public float Float () {

		float rand = (float) generator.NextDouble ();
		return rand;

	}

	public float Float ( float min, float max ) {

		return min + Float () * ( max - min );

	}

	public int Int ( int min, int max ) {

		return Mathf.FloorToInt ( Float ( min, max ) );

	}

	public Vector3 Vector3 () {

		float randX = Float(),
			randY = Float(),
			randZ = Float();

		Vector3 v3 = new Vector3 ( randX, randY, randZ ).normalized;

		return v3;

	}

	public Vector3 CircleVector3 () {

		float randX = Float () - .5f,
			randY = Float () - .5f,
			randZ = Float () - .5f;

		Vector3 v3 = new Vector3 ( randX, randY, randZ ).normalized;

		return v3;

	}

}
