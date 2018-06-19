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

		private Rigidbody m_RB;
		public Rigidbody RB {
			get {
				if ( m_RB == null ) m_RB = GetComponent<Rigidbody> ();
				return m_RB;
			}
		}

		public Action< object > motion;

		public Vector3 delta;

		public Transform view;

		public static Rand rand;

		protected void Start () {

			if ( motion == null ) {

				InitMotion ();

			}

			if ( life >= 0 ) {

				StartCoroutine ( Kill ( life ) );

			}

			if ( rand == null ) {

				rand = gameObject.AddComponent<Rand> ();

			}

			if ( templateView != null )
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

			Vector3 mForce = rand.CircleV3 () * particle.speed;

			particle.RB.AddForce ( mForce, ForceMode.Force );

		}

	}
}