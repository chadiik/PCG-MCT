using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace pcg {

	[Serializable]
	public class SplineNode {

		public Vector3 position;
		public Vector3 direction;
		private bool m_NeedsUpdate = true;

		public SplineNode ( Vector3 position, Vector3 direction ) {
			SetPosition ( position );
			SetDirection ( direction );
		}

		public void SetPosition ( Vector3 p ) {
			if ( !position.Equals ( p ) ) {
				position.x = p.x;
				position.y = p.y;
				position.z = p.z;

				m_NeedsUpdate = true;
			}
		}

		public void SetDirection ( Vector3 d ) {
			if ( !direction.Equals ( d ) ) {
				direction.x = d.x;
				direction.y = d.y;
				direction.z = d.z;

				m_NeedsUpdate = true;
			}
		}

		public bool NeedsUpdate {
			get {
				return m_NeedsUpdate;
			}
		}

		public void Update () {

			m_NeedsUpdate = false;

		}

	}
}