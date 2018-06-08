using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {

	[ExecuteInEditMode]
	public class ParticleAffectorRB : MonoBehaviour {

		public float life = -1;
		public float influenceStartRadius = 0;
		public float influenceSpan = 1;
		public float strength = 1;
		public float gravityPull = 1;
		public AnimationCurve influence;

		public Rand rand;

		private float m_Strength;
		private float m_CycleOffset;
		private float m_BirthTime;
		private float m_Age;

		protected void Start () {

			float s = strength > 0 ? ( influenceStartRadius + influenceSpan ) * 2f : ( influenceStartRadius + influenceSpan ) * 2f;
			transform.localScale = new Vector3 ( s, s, s );

			m_Strength = strength;
			m_CycleOffset = rand.Float () * Mathf.PI * 2f;

			if ( life >= 0 ) {

				StartCoroutine ( Kill ( life ) );

			}

			m_BirthTime = Time.time;
			m_Age = 0;

			if ( rand == null ) {

				rand = gameObject.AddComponent<Rand> ();

			}

		}

		public IEnumerator Kill ( float life ) {

			yield return new WaitForSeconds ( life );

			Destroy ( gameObject );

		}

		protected void FixedUpdate () {

			float t = Time.time;

			if ( life >= 0 ) {

				m_Age = t - m_BirthTime;

				float power = ( 1 - m_Age / life );
				power = Mathf.Pow( power, .5f );
				strength = m_Strength * power;

				if ( float.IsNaN ( strength ) )
					strength = 0;

				float s = strength > 0 ? ( influenceStartRadius + influenceSpan ) * 1f : ( influenceStartRadius + influenceSpan ) * 1f;
				s *= Mathf.Abs ( strength );

				transform.localScale = new Vector3 ( s, s, s );

			}

		}

	}
}
