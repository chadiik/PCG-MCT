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

		public BoidsFlock flock;

		private Vector3 currentHeading = new Vector3 ( 0, 0, 0 );
		private Vector3 newHeading = new Vector3 ( 0, 0, 0 );

		private Vector3 separationVector = new Vector3 ( 0, 0, 0 );
		private Vector3 alignmentVector = new Vector3 ( 0, 0, 0 );
		private Vector3 cohesionVector = new Vector3 ( 0, 0, 0 );

		private Heading[] headings;
		private int skipCalcFor;
		private Collider[] neighbours;

		protected void Start () {

			currentHeading = transform.forward;

			headings = new Heading[] { new Heading (), new Heading (), new Heading (), new Heading (), new Heading () };

			skipCalcFor = UnityEngine.Random.Range ( 0, 3 );

			neighbours = new Collider [ flock.boids.Count / 4 ];

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
			if ( --skipCalcFor < 0 ) {
				PhyUpdate ( out separationVector, out alignmentVector, out cohesionVector );
				skipCalcFor = 2;
			}
			/*
			separationVector = Separation ();
			alignmentVector = Alignment ();
			cohesionVector = Cohesion ();
			*/

			// collect headings, but only use if non-zero

			if ( flock.currentHeadingWeight != 0.0f )
				headings[ 0 ].Set ( transform.rotation, flock.currentHeadingWeight );
			else
				headings[ 0 ].Reset ();

			if ( ! separationVector.NearZero() )
				headings[ 1 ].Set ( Quaternion.LookRotation ( separationVector ), flock.separationWeight );
			else
				headings[ 1 ].Reset ();

			if ( ! alignmentVector.NearZero () )
				headings[ 2 ].Set ( Quaternion.LookRotation ( alignmentVector ), flock.alignmentWeight );
			else
				headings[ 2 ].Reset ();

			if ( ! cohesionVector.NearZero () )
				headings[ 3 ].Set ( Quaternion.LookRotation ( cohesionVector ), flock.cohesionWeight );
			else
				headings[ 3 ].Reset ();

			if ( flock.headToOriginWeight > 0f ) {

				headings[ 4 ].Set ( Quaternion.LookRotation ( flock.origin - transform.position ), flock.headToOriginWeight );

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

			transform.rotation = Quaternion.Lerp ( transform.rotation, newHeading, flock.rotationSmooth );
			transform.position += transform.forward * flock.speed;

		}

		#region Regular search
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

		#endregion

		#region Physics search
		void PhyUpdate ( out Vector3 separationVector, out Vector3 alignmentVector, out Vector3 cohesionVector) {

			separationVector = new Vector3 ( 0, 0, 0 );
			alignmentVector = new Vector3 ( 0, 0, 0 );
			cohesionVector = new Vector3 ( 0, 0, 0 );

			Vector3 position = transform.position, neighbourPosition;

			int numNeighbours = Physics.OverlapSphereNonAlloc ( position, flock.nearRadius, neighbours, flock.agentsLayer );

			if ( numNeighbours > 1 ) {

				for ( int i = 0; i < numNeighbours; i++ ) {

					Collider neighbour = neighbours[ i ];

					if ( neighbour.gameObject == this.gameObject ) continue;

					neighbourPosition = neighbour.transform.position;
					Vector3 differenceVector = position - neighbourPosition;
					float magnitudeSquared = differenceVector.sqrMagnitude;
					float weightedMagnitude = 1.0f / magnitudeSquared;

					// Separation
					differenceVector = Vector3.Scale ( differenceVector, new Vector3 ( weightedMagnitude, weightedMagnitude, weightedMagnitude ) );
					separationVector += differenceVector;

					// Alignment
					Vector3 otherHeading = neighbour.transform.forward;
					alignmentVector = Vector3.Slerp ( alignmentVector, otherHeading, weightedMagnitude );

					// Cohesion
					cohesionVector += neighbourPosition;

				}

				cohesionVector /= ( float ) numNeighbours;
				cohesionVector -= position;

			}

		}

		#endregion

	}
}