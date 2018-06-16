using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {

	public class AffectedParticleRB : ParticleRB {

		[ Header ( "Affected" ) ]
		public float effectStrength = 1;

		public static List<ParticleAffectorRB> commonAffectors = new List<ParticleAffectorRB>();
		public List<ParticleAffectorRB> affectors;
		
		protected override void InitMotion () {

			motion = AffectedBrownian;

		}

		public static void AffectedBrownian ( object value ) {

			AffectedParticleRB particle = value as AffectedParticleRB;

			( ( Action<ParticleRB> )ParticleRB.Brownian ).Invoke ( particle );

			//Vector3 affectorsPull = AggregateAffectors ( particle.transform.position, particle.rb.mass, particle.affectors ) * particle.effectStrength;
			Vector3 affectorsPull = AggregateAffectorsInfCurve ( particle.transform.position, particle.affectors ) * particle.effectStrength;
			affectorsPull += AggregateAffectorsInfCurve ( particle.transform.position, commonAffectors ) * particle.effectStrength;
			particle.RB.AddForce ( affectorsPull, ForceMode.Force );

		}

		public static Vector3 AggregateAffectors ( Vector3 position, float mass, List<ParticleAffectorRB> affectors ) {

			Vector3 extra = new Vector3 ();

			for ( int i = 0; i < affectors.Count; i++ ) {

				ParticleAffectorRB affector = affectors[ i ];

				if ( affector == null ) {

					affectors.RemoveAt ( i );
					i--;
					continue;

				}

				Vector3 delta = affector.transform.position - position;
				float distanceSquared = delta.sqrMagnitude;

				extra += delta.normalized * ( ( mass * affector.gravityPull ) / distanceSquared );

			}

			return extra;

		}

		public static Vector3 AggregateAffectorsInfCurve ( Vector3 position, List<ParticleAffectorRB> affectors ) {

			Vector3 extra = new Vector3 ();

			for ( int i = 0; i < affectors.Count; i++ ) {

				ParticleAffectorRB affector = affectors[ i ];

				if ( affector == null ) {

					affectors.RemoveAt ( i );
					i--;
					continue;

				}

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

	}
}