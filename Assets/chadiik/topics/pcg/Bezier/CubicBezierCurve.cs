using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace pcg {

	[Serializable]
	public class CubicBezierCurve {

		private int m_StepCount;
		private float m_TStep;
		private bool m_NeedsUpdate = true;

		public SplineNode n1, n2;

		/// <summary>
		/// Length of the curve in world unit.
		/// </summary>
		public float Length { get; private set; }
		private List<CurveSample> m_Samples;

		public int StepCount {
			get {

				return m_StepCount;

			}
			set {

				if ( m_StepCount != value ) m_NeedsUpdate = true;

				m_StepCount = value;
				m_TStep = 1f / (float)m_StepCount;

				if ( m_Samples == null ) m_Samples = new List<CurveSample> ( m_StepCount );
				else m_Samples.Clear ();

				m_Samples.Capacity = m_StepCount;

			}
		}

		
		public CubicBezierCurve ( SplineNode n1, SplineNode n2, int stepCount ) {
			this.n1 = n1;
			this.n2 = n2;

			StepCount = stepCount;

			m_NeedsUpdate = true;
		}

		/// <summary>
		/// Change the start node of the curve.
		/// </summary>
		/// <param name="n1"></param>
		public void ConnectStart ( SplineNode n1 ) {

			this.n1 = n1;
			m_NeedsUpdate = true;

		}

		/// <summary>
		/// Change the end node of the curve.
		/// </summary>
		/// <param name="n2"></param>
		public void ConnectEnd ( SplineNode n2 ) {

			this.n2 = n2;
			m_NeedsUpdate = true;

		}

		/// <summary>
		/// Convinient method to get the third control point of the curve, as the direction of the end spline node indicates the starting tangent of the next curve.
		/// </summary>
		/// <returns></returns>
		public Vector3 GetInverseDirection () {

			return ( 2 * n2.position ) - n2.direction;

		}

		/// <summary>
		/// Returns point on curve at given time. Time must be between 0 and 1.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Vector3 GetLocation ( float t ) {

			t = Mathf.Clamp01 ( t );

			float omt = 1f - t;
			float omt2 = omt * omt;
			float t2 = t * t;

			Vector3 location =
				n1.position * ( omt2 * omt ) +
				n1.direction * ( 3f * omt2 * t ) +
				GetInverseDirection () * ( 3f * omt * t2 ) +
				n2.position * ( t2 * t );

			return location;

		}

		/// <summary>
		/// Returns tangent of curve at given time. Time must be between 0 and 1.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Vector3 GetTangent ( float t ) {

			t = Mathf.Clamp01 ( t );

			float omt = 1f - t;
			float omt2 = omt * omt;
			float t2 = t * t;

			Vector3 tangent =
				n1.position * ( -omt2 ) +
				n1.direction * ( 3 * omt2 - 2 * omt ) +
				GetInverseDirection () * ( -3 * t2 + 2 * t ) +
				n2.position * ( t2 );

			return tangent.normalized;

		}

		private void ComputePoints () {

			m_Samples.Clear ();
			Length = 0;
			Vector3 previousPosition = GetLocation ( 0 );

			for ( float t = 0; t < 1; t += m_TStep ) {

				CurveSample sample = new CurveSample ();
				sample.location = GetLocation ( t );
				sample.tangent = GetTangent ( t );
				Length += Vector3.Distance ( previousPosition, sample.location );
				sample.distance = Length;

				previousPosition = sample.location;
				m_Samples.Add ( sample );

			}

			Debug.Log ( m_Samples.Count );

			CurveSample lastSample = new CurveSample ();
			lastSample.location = GetLocation ( 1 );
			lastSample.tangent = GetTangent ( 1 );
			Length += Vector3.Distance ( previousPosition, lastSample.location );
			lastSample.distance = Length;
			m_Samples.Add ( lastSample );

		}

		private CurveSample getCurvePointAtDistance ( float d ) {

			d = Mathf.Clamp ( d, 0, Length );

			CurveSample previous = m_Samples[ 0 ];
			CurveSample next = m_Samples[ m_StepCount - 1 ];

			for(int i = 0; i < m_StepCount; i++){

				CurveSample cp = m_Samples[i];

				if ( cp.distance >= d ) {

					next = cp;
					break;

				}

				previous = cp;

			}

			if ( next == null ) {

				throw new Exception ( "Can't find curve samples." );

			}

			float t = next == previous ? 0 : ( d - previous.distance ) / ( next.distance - previous.distance );

			CurveSample res = new CurveSample ();
			res.distance = d;
			res.location = Vector3.Lerp ( previous.location, next.location, t );
			res.tangent = Vector3.Lerp ( previous.tangent, next.tangent, t ).normalized;

			return res;

		}

		/// <summary>
		/// Returns point on curve at distance. Distance must be between 0 and curve length.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public Vector3 GetLocationAtDistance ( float d ) {

			return getCurvePointAtDistance ( d ).location;

		}

		/// <summary>
		/// Returns tangent of curve at distance. Distance must be between 0 and curve length.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public Vector3 GetTangentAtDistance ( float d ) {

			return getCurvePointAtDistance ( d ).tangent;

		}

		public bool NeedsUpdate {
			get {
				return m_NeedsUpdate || n1.NeedsUpdate || n2.NeedsUpdate;
			}
		}

		public void Update () {

			if ( NeedsUpdate ) {

				m_NeedsUpdate = false;
				
				n1.Update ();
				n2.Update ();
				ComputePoints ();

			}

		}

		private int m_IterSampleIndex = 0;
		public CurveSample NewIteration () {

			m_IterSampleIndex = 0;
			CurveSample sample = Iterate ();

			return sample;

		}

		public CurveSample Iterate () {

			if ( m_IterSampleIndex >= m_StepCount )
				return null;

			CurveSample sample = m_Samples[ m_IterSampleIndex++ ];
			return sample;

		}

		public class CurveSample {

			public Vector3 location;
			public Vector3 tangent;
			public float distance;

		}

		/// <summary>
		/// Convenient method that returns a quaternion used rotate an object in the tangent direction, considering Y-axis as up vector.
		/// </summary>
		/// <param name="Tangent"></param>
		/// <returns></returns>
		public static Quaternion GetRotationFromTangent ( Vector3 Tangent, Vector3 up ) {

			if ( Tangent == Vector3.zero )
				return Quaternion.identity;

			return Quaternion.LookRotation ( Tangent, Vector3.Cross ( Tangent, Vector3.Cross ( up, Tangent ).normalized ) );

		}
	}
}