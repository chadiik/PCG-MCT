using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace pcg {
	public class BoidsFlock : MonoBehaviour {

		public Boid template;
		[HideInInspector]
		public List<Boid> boids = new List<Boid> ();
		public Pattern spawnPattern;
		public Transform originTarget;
		[HideInInspector]
		public Vector3 origin;
		public LayerMask agentsLayer;

		[Header("Properties")]
		public float separationWeight = 1;
		public float alignmentWeight = 1;
		public float cohesionWeight = 1;
		public float headToOriginWeight = 1;
		public float speed = 1.0f;
		public float currentHeadingWeight = 1.0f;
		public float rotationSmooth = .1f;
		public float nearRadius = 10f;

		[Space]
		[Header("Preset")]
		public FlockPreset preset;
		public string newPresetName;
		public string presetsPath = "Assets/";
		public bool savePreset = false;

		protected void Start () {

			if ( preset != null )
				ApplyPreset ( preset );

			if ( spawnPattern != null ) {

				foreach ( Vector3 v3 in spawnPattern ) {

					InstantiateBoid ( v3 );

				}

			}

		}

		public void CleanList () {

			for(int i = 0; i < boids.Count; i++ ) {

				if(boids[i] == null ) {

					boids.RemoveAt ( i );
					i--;

				}

			}

		}

		public Boid InstantiateBoid ( Vector3 v3 ) {

			Boid boid = Instantiate<Boid> ( template, transform );
			boid.flock = this;
			boid.transform.position = v3;

			boids.Add ( boid );

			return boid;

		}

		private void ApplyPreset ( FlockPreset preset ) {

			separationWeight = preset.separationWeight;
			alignmentWeight = preset.alignmentWeight;
			cohesionWeight = preset.cohesionWeight;
			headToOriginWeight = preset.headToOriginWeight;
			speed = preset.speed;
			currentHeadingWeight = preset.currentHeadingWeight;
			rotationSmooth = preset.rotationSmooth;
			nearRadius = preset.nearRadius;

		}

		protected void FixedUpdate () {

			origin = originTarget.position;

		}

		protected void Update () {

			if ( savePreset ) {

				savePreset = false;
				CreatePreset ();

			}

		}

		public void TransitionToPreset( FlockPreset targetPreset, float duration, Func<float, float, float, float> lerp = null ) {

			StartCoroutine ( TransitionToPresetCoroutine ( targetPreset, duration, lerp != null ? lerp : Mathf.Lerp ) );

		}

		private IEnumerator TransitionToPresetCoroutine ( FlockPreset targetPreset, float duration, Func<float, float, float, float> lerp ) {

			float startTime = Time.time;

			float separationWeight = this.separationWeight;
			float alignmentWeight = this.alignmentWeight;
			float cohesionWeight = this.cohesionWeight;
			float headToOriginWeight = this.headToOriginWeight;
			float speed = this.speed;
			float currentHeadingWeight = this.currentHeadingWeight;
			float rotationSmooth = this.rotationSmooth;
			float nearRadius = this.nearRadius;

			float t = 0f;

			while ( t < 1f ) {

				this.separationWeight = lerp ( separationWeight, targetPreset.separationWeight, t );
				this.alignmentWeight = lerp ( alignmentWeight, targetPreset.alignmentWeight, t );
				this.cohesionWeight = lerp ( cohesionWeight, targetPreset.cohesionWeight, t );
				this.headToOriginWeight = lerp ( headToOriginWeight, targetPreset.headToOriginWeight, t );
				this.speed = lerp ( speed, targetPreset.speed, t );
				this.currentHeadingWeight = lerp ( currentHeadingWeight, targetPreset.currentHeadingWeight, t );
				this.rotationSmooth = lerp ( rotationSmooth, targetPreset.rotationSmooth, t );
				this.nearRadius = lerp ( nearRadius, targetPreset.nearRadius, t );

				t = ( Time.time - startTime ) / duration;
				yield return new WaitForFixedUpdate ();

			}

			ApplyPreset ( targetPreset );

		}

		public void CreateBoidsOverDuration( Pattern pattern, float duration ) {
			StartCoroutine ( CreateBoidsOverDurationCoroutine ( pattern, duration ) );
		}

		private IEnumerator CreateBoidsOverDurationCoroutine ( Pattern pattern, float duration ) {

			float delay = duration / pattern.Count;

			foreach ( Vector3 v3 in pattern ) {

				InstantiateBoid ( v3 );
				yield return new WaitForSeconds ( delay );

			}

		}

		private void CreatePreset () {

			FlockPreset newPreset = ScriptableObject.CreateInstance<FlockPreset>();

			newPreset.separationWeight = separationWeight;
			newPreset.alignmentWeight = alignmentWeight;
			newPreset.cohesionWeight = cohesionWeight;
			newPreset.headToOriginWeight = headToOriginWeight;
			newPreset.speed = speed;
			newPreset.currentHeadingWeight = currentHeadingWeight;
			newPreset.rotationSmooth = rotationSmooth;
			newPreset.nearRadius = nearRadius;

			string path = presetsPath + "/Presets";
			if ( !AssetDatabase.IsValidFolder ( path ) )
				AssetDatabase.CreateFolder ( presetsPath, "Presets" );

			string newName = string.IsNullOrEmpty( newPresetName ) ? "Flock" + DateTime.Now.Ticks : newPresetName;
			AssetDatabase.CreateAsset ( newPreset, path + "/" + newName + ".asset" );

		}

	}
}