using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace pcg {
	public class BoidsFlock : MonoBehaviour {

		public Boid template;
		[HideInInspector]
		public List<Boid> boids;
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

			boids = new List<Boid> ();

			foreach ( Vector3 v3 in spawnPattern ) {

				Boid boid = Instantiate<Boid> ( template, transform );
				boid.flock = this;
				boid.transform.position = v3;

				boids.Add ( boid );
				
			}

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