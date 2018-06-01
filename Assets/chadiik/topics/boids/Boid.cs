using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Boids artificial life algorithm http://en.wikipedia.org/wiki/Boids
separation: steer to avoid crowding local flockmates
alignment: steer towards the average heading of local flockmates
cohesion: steer to move toward the average position (center of mass) of local flockmates
*/

namespace pcg {
	public class Boid : MonoBehaviour {

		public float separationWeight = 1;
		public float alignmentWeight = 1;
		public float cohesionWeight = 1;
		public float headToOriginWeight = 1;

		public BoidsFlock flock;

		public float speed = 1.0f;
		public float currentHeadingWeight = 1.0f;

		private Vector3 currentHeading = new Vector3 ( 0, 0, 0 );
		private Vector3 newHeading = new Vector3 ( 0, 0, 0 );

		private Vector3 separationVector = new Vector3 ( 0, 0, 0 );
		private Vector3 alignmentVector = new Vector3 ( 0, 0, 0 );
		private Vector3 cohesionVector = new Vector3 ( 0, 0, 0 );

		private Heading[] headings;

		protected void Start () {

			currentHeading = transform.forward;

			headings = new Heading[] { new Heading (), new Heading (), new Heading (), new Heading (), new Heading () };

		}

		class Heading {

			public Quaternion rotation;
			public float weight;

			public Heading () {

			}

			public void Reset () {

				this.rotation = Quaternion.identity;
				this.weight = 0;

			}

			public void Set ( Quaternion rotation, float weight ) {

				this.rotation = rotation;
				this.weight = weight;

			}
			
		};

		protected void FixedUpdate () {

			// think
			separationVector = Separation ();
			alignmentVector = Alignment ();
			cohesionVector = Cohesion ();

			// collect headings, but only use if non-zero

			if ( currentHeadingWeight != 0.0f )
				headings[ 0 ].Set ( transform.rotation, currentHeadingWeight );
			else
				headings[ 0 ].Reset ();

			if ( separationVector != Vector3.zero )
				headings[ 1 ].Set ( Quaternion.LookRotation ( separationVector ), separationWeight );
			else
				headings[ 1 ].Reset ();

			if ( alignmentVector != Vector3.zero )
				headings[ 2 ].Set ( Quaternion.LookRotation ( alignmentVector ), alignmentWeight );
			else
				headings[ 2 ].Reset ();

			if ( cohesionVector != Vector3.zero )
				headings[ 3 ].Set ( Quaternion.LookRotation ( cohesionVector ), cohesionWeight );
			else
				headings[ 3 ].Reset ();

			if ( headToOriginWeight > 0f ) {

				headings[ 4 ].Set ( Quaternion.LookRotation ( flock.origin - transform.position ), headToOriginWeight );

			}

			// normalize weights so they add up to one
			float totalWeight = 0.0f;

			foreach ( Heading heading in headings )
				totalWeight += heading.weight;

			foreach ( Heading heading in headings )
				heading.weight /= totalWeight;

			totalWeight = 0.0f;
			var newHeading = Quaternion.identity;

			foreach ( Heading heading in headings ) {

				totalWeight += heading.weight;
				newHeading = Quaternion.Slerp ( newHeading, heading.rotation, heading.weight / totalWeight );

			}

			transform.rotation = newHeading;
			transform.position += transform.forward * speed;

		}

		// separation: steer to avoid crowding local flockmates
		Vector3 Separation () {

			Vector3 resultVector = new Vector3 ( 0, 0, 0 );

			List<Boid> boids = flock.boids;

			for ( int i = 0, numBoids = boids.Count; i < numBoids ; i++ ) {

				Boid neighbour = boids[ i ];
				if ( neighbour == this ) continue;

				Vector3 differenceVector = transform.position - neighbour.transform.position;

				float magnitudeSquared = differenceVector.sqrMagnitude;

				if ( magnitudeSquared < 100f * 100f ) {

					float weightedMagnitude = 1.0f / magnitudeSquared;
					differenceVector = Vector3.Scale ( differenceVector, new Vector3 ( weightedMagnitude, weightedMagnitude, weightedMagnitude ) );
					resultVector = resultVector + differenceVector;

				}

			}

			return resultVector;
		}

		// alignment: steer towards the average heading of local flockmates
		Vector3 Alignment () {

			Vector3 resultVector = new Vector3 ( 0, 0, 0 );

			List<Boid> boids = flock.boids;

			for ( int i = 0, numBoids = boids.Count; i < numBoids; i++ ) {

				Boid neighbour = boids[ i ];
				if ( neighbour == this ) continue;

				float distanceSquared = ( transform.position - neighbour.transform.position ).sqrMagnitude;
				Vector3 otherHeading = neighbour.transform.forward;

				resultVector = Vector3.Slerp ( resultVector, otherHeading, ( 1.0f / distanceSquared ) );

			}

			return resultVector;
		}

		// cohesion: steer to move toward the average position (center of mass) of local flockmates
		Vector3 Cohesion () {

			Vector3 resultVector = new Vector3 ( 0, 0, 0 );

			List<Boid> boids = flock.boids;

			int numBoids = boids.Count;

			for ( int i = 0; i < numBoids; i++ ) {

				Boid neighbour = boids[ i ];
				if ( neighbour == this ) continue;

				resultVector = resultVector + neighbour.transform.position;

			}

			resultVector = resultVector / ( float )numBoids;

			resultVector = resultVector - transform.position;

			return resultVector;

		}

		/*
		void GetFlock () {

			Boid[] boids = FindObjectsOfType ( typeof ( Boid ) ) as Boid[];

			// add all boids except for self
			for ( int i = 0; i < boids.Length; i++ ) {

				int id1 = boids[ i ].GetInstanceID ();
				Boid boid = gameObject.GetComponent ( typeof ( Boid ) ) as Boid;
				int id2 = boid.GetInstanceID ();

				if ( id1 != id2 ) {

					flock.Add ( boids[ i ] );
					
				}

			}

		}
		*/

	}
}