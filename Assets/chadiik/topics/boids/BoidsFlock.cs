using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {
	public class BoidsFlock : MonoBehaviour {

		public Boid template;
		public List<Boid> boids;
		public Pattern spawnPattern;
		public Vector3 origin;

		protected void Start () {

			boids = new List<Boid> ();

			foreach ( Vector3 v3 in spawnPattern ) {

				Boid boid = Instantiate<Boid> ( template, transform );
				boid.flock = this;
				boid.transform.position = v3;

				boids.Add ( boid );
				
			}

		}

	}
}