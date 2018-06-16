using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {
	[System.Serializable]
	public class Direction {

		public Vector3 vector;
		public float coneRadius;

		private Rand m_Rand;
		public Rand Rand {
			set {
				m_Rand = value;
			}
			get {
				if(m_Rand == null ) {
					m_Rand = Rand.Instance;
				}
				return m_Rand;
			}
		}

		public Vector3 GetRandom () {

			Vector3 direction = Rand.RangedDirection ( vector, coneRadius );
			return direction;

		}

	}
}