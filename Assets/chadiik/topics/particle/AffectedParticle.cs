using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {

	public class AffectedParticle : Particle {

		[ Header ( "Affected" ) ]
		public float effectStrength = 1;

		public List<ParticleAffector> affectors;
		
		protected override void InitMotion () {

			motion = AffectedBrownian;

		}

		public static void AffectedBrownian ( dynamic value ) {

			AffectedParticle particle = value as AffectedParticle;
			Transform transform = particle.transform;

			Vector3 previousPosition = transform.position;

			( ( Action<Particle> )Particle.Brownian ).Invoke ( particle );

			Vector3 position = transform.position;

			Vector3 targetPosition = previousPosition + AggregateAffectors ( position, particle.affectors );

			position = Vector3.Lerp ( position, targetPosition, particle.effectStrength );

			transform.position = position;

		}

		public static Vector3 AggregateAffectors ( Vector3 position, List<ParticleAffector> affectors ) {

			Vector3 extra = new Vector3 ();

			for ( int i = 0, numAffectors = affectors.Count; i < numAffectors; i++ ) {

				ParticleAffector affector = affectors[i];
				Vector3 delta = affector.transform.position - position;
				float distanceSquared = delta.sqrMagnitude;

				float minDistanceSquared = affector.influenceStartRadius * affector.influenceStartRadius;
				float maxDistanceSquared = ( affector.influenceStartRadius + affector.influenceSpan ) * ( affector.influenceStartRadius + affector.influenceSpan );
				if ( distanceSquared < maxDistanceSquared && distanceSquared > minDistanceSquared ) {

					float distance = Mathf.Sqrt ( distanceSquared );

					float t = ( distance - affector.influenceStartRadius ) / affector.influenceSpan;
					float pullForce = affector.influence.Evaluate ( t ) * affector.strength;

					Vector3 pull = delta / distance * pullForce; // Optimize with calculated values

					extra += pull;

				}

			}

			return extra;

		}

		public static void AffectedBrownianTest ( dynamic value ) {

			AffectedParticle particle = value as AffectedParticle;

			(( Action < Particle > )Particle.Brownian).Invoke ( particle );

			//
			float tLength = 4;
			float a = Time.time;
			Vector3 targetPosition = new Vector3(
				Mathf.Cos(a) * tLength,
				Mathf.Sin(a) * tLength,
				Mathf.Cos(a) * tLength
			);

			Transform transform = particle.transform;

			Vector3 position = transform.position;

			position = Vector3.Lerp ( position, targetPosition, particle.effectStrength );

			transform.position = position;

		}

	}
}