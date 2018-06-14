using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {

	public class ParticleRB : MonoBehaviour {

		public Transform templateView;
		public bool d2 = false;
		public float speed = 1f;
		public float life = -1;

		public Rigidbody rb;

		public Action< object > motion;

		public Vector3 delta;

		public Transform view;

		public static Rand rand;

		private float m_Age;

		protected void Start () {

			if ( rb == null ) {

				rb = GetComponent<Rigidbody> ();

			}

			if ( motion == null ) {

				InitMotion ();

			}

			if ( life >= 0 ) {

				StartCoroutine ( Kill ( life ) );

			}

			if ( rand == null ) {

				rand = gameObject.AddComponent<Rand> ();

			}

			m_Age = 0;

			view = Instantiate ( templateView, transform );

		}

		public IEnumerator Kill( float life ){

			yield return new WaitForSeconds( life );

			Destroy ( gameObject );

		}

		protected void FixedUpdate () {

			Vector3 previousPosition = transform.position;

			motion.Invoke ( this );

			if ( d2 ) {

				Vector3 position = transform.position;
				position.z = 0;

				transform.position = position;

			}

		}

		protected virtual void InitMotion () {

			motion = Brownian;

		}


		/////

		public static void Brownian ( object value ) {

			ParticleRB particle = value as ParticleRB;

			Vector3 mForce = rand.CircleVector3 () * particle.speed;

			particle.rb.AddForce ( mForce, ForceMode.Force );

		}

	}
}