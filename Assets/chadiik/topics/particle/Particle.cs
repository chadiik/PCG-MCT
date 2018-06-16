using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {

	public class Particle : MonoBehaviour {

		public Transform templateView;
		public bool drawLine = false;
		public bool d2 = false;
		public float speed = 1f;
		public float slerp = 1f;
		public float velocityMoment = 0f;
		public float drag = 0f;
		public Action< object > motion;

		public Vector3 delta;

		public Transform view;

		public static Rand rand;

		private Vector3 m_TargetPosition;

		protected void Start () {

			if ( motion == null ) {

				InitMotion ();

			}

			view = Instantiate ( templateView, transform );

			if ( drawLine ) {

				DrawScreenLine.StrokeLineFrom ( transform.position );

			}

			if ( rand == null ) {

				rand = gameObject.AddComponent<Rand> ();

			}

		}

		protected void FixedUpdate () {

			Vector3 previousPosition = transform.position;

			motion.Invoke ( this );

			if ( drawLine ) {

				DrawScreenLine.StrokeLineTo ( transform.position );

			}

			delta += (transform.position - previousPosition) * velocityMoment;
			delta *= drag;

			transform.position = transform.position + delta;

			if ( d2 ) {

				Vector3 position = transform.position;
				position.z = 0;

				transform.position = position;

			}

			m_TargetPosition = transform.position;

			transform.position = Vector3.Slerp ( previousPosition, m_TargetPosition, slerp );

		}

		protected virtual void InitMotion () {

			motion = Brownian;

		}


		/////

		public static void Brownian ( object value ) {

			Particle particle = value as Particle;

			Transform transform = particle.transform;

			Vector3 position = transform.position;
			position += rand.CircleV3 () * particle.speed * Time.deltaTime;

			transform.position = position;

		}

	}
}