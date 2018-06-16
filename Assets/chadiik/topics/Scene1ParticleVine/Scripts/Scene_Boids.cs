using pcg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uween;

public class Scene_Boids : MonoBehaviour {

	public enum Step { Idle, Attraction, AttractionUpDown, SettleCohesion };
	public enum Parallel { Idle, LerpToSeparation };

	public Step step;
	public Parallel parallel;
	public BoidsFlock flock;

	[System.Serializable]
	public class P {
		[Header("Transition")]
		[Space]
		[Header("Separation")]
		public FlockPreset separationPreset;
		public float separationTransitionDuration = 3f;

		[Header("Additional boids")]
		public Pattern additionalBoidsPattern;
		public float additionalBoidsCreateDuration;

		[Header("Fish school 1")]
		public FlockPreset fishSchool1Preset;
		public float fishSchool1TransitionDuration;
		public Transform fish1Template;
	}
	public P p;

		[Header("Mock")]
	public bool mockPreviousStep = true;
	public Pattern mockPattern;
	public GameObject[] mockDestroy;

	protected void Awake () {

		if ( !mockPreviousStep ) {

			gameObject.SetActive ( false );

		}
		else {

			foreach ( GameObject go in mockDestroy )
				DestroyImmediate ( go );

			flock.spawnPattern = mockPattern;

		}

	}

	protected void Start () {

		StartCoroutine ( ScenarioCoroutine () );
		StartCoroutine ( ParallelCoroutine () );

	}

	private IEnumerator ScenarioCoroutine () {

		while ( isActiveAndEnabled )
			yield return ScenarioExecution ();

	}

	private IEnumerator ParallelCoroutine () {

		while ( isActiveAndEnabled )
			yield return ParallelExecution ();

	}

	private IEnumerator ScenarioExecution () {

		Vector3 newParticleDirection;
		float time, t;

		switch ( step ) {
			case Step.Attraction: {

					yield return null;
					break;
				}

			case Step.AttractionUpDown: {

					Vector3 origin = flock.originTarget.position;
					float amplitude = 0f;

					while ( step == Step.AttractionUpDown ) {
						amplitude = AttractionUpDown ( origin, amplitude );

						yield return new WaitForFixedUpdate ();

					}

					step = Step.Idle;
					break;

				}

			case Step.SettleCohesion: {

					Tween.DestroyAll ( flock.originTarget.gameObject );
					Scene_Camera.Instance.SmoothLookAt ( flock.originTarget, 2f, true );
					flock.TransitionToPreset ( p.fishSchool1Preset, p.fishSchool1TransitionDuration );
					ConvertBoidsToTemplate ( p.fish1Template, p.fishSchool1TransitionDuration );

					step = Step.Idle;
					break;
				}
		}
	}

	private void ConvertBoidsToTemplate ( Transform template, float duration ) {

		flock.CleanList ();
		StartCoroutine ( ConvertBoidsToTemplateCoroutine ( template, duration ) );
	}

	private IEnumerator ConvertBoidsToTemplateCoroutine ( Transform template, float duration ) {

		float delay = duration / flock.boids.Count;

		int countNulls = 0;
		foreach ( Boid boid in flock.boids ) {

			if ( boid != null ) {

				//SwitchMeshView.Apply ( boid.GetComponentInChildren<MeshFilter>().gameObject, template.gameObject );
				GameObject original = boid.GetComponentInChildren<MeshFilter>().gameObject;
				Instantiate ( template, original.transform.parent );
				Destroy ( original );
				Scene_Particle.AppearZoom ( boid.gameObject, 0f );

				yield return new WaitForSeconds ( delay );

			}
			else {
				countNulls++;
			}

		}

		Debug.LogFormat ( "Null boids = {0}", countNulls );

	}

	private IEnumerator ParallelExecution () {

		Vector3 newParticleDirection;
		float time, t;

		switch ( parallel ) {
			case Parallel.LerpToSeparation: {
					flock.TransitionToPreset ( p.separationPreset, p.separationTransitionDuration );
					flock.CreateBoidsOverDuration ( p.additionalBoidsPattern, p.additionalBoidsCreateDuration );

					parallel = Parallel.Idle;
					break;
				}

			default: {

					break;
				}
		}

		yield return null;

	}

	private float AttractionUpDown ( Vector3 origin, float amplitude ) {

		float time = Time.time;
		float yOffset = Mathf.Sin(time) * Mathf.Min(3.5f, amplitude += .05f);
		Vector3 newPosition = origin;
		newPosition.y += yOffset;
		flock.originTarget.position = newPosition;

		return amplitude;

	}

	private void AttractionUpDownTween () {

		Vector3 origin = flock.originTarget.position;

		TweenY.Add ( flock.originTarget.gameObject, 4f, origin.y - 10f ).EaseInOutSine ().Then ( () => {
			TweenY.Add ( flock.originTarget.gameObject, 4f, origin.y + 10f ).EaseInOutSine ().Then ( AttractionUpDownTween );
		} );

	}
}
