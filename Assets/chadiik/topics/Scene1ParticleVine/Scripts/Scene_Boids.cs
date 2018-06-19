using pcg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uween;

public class Scene_Boids : MonoBehaviour {

	public static Scene_Boids instance;

	public enum Step { Idle, Attraction, AttractionUpDown, SettleCohesion, School2, Birds };
	public enum Parallel { Idle, LerpToSeparation };

	public Step step;
	public Parallel parallel;
	public BoidsFlock flock;

	[System.Serializable]
	public class P {
		public CameraControllerPreset camView;
		[Header("Transition")]
		[Space]
		[Header("Separation")]
		public FlockPreset separationPreset;
		public float separationTransitionDuration = 3f;

		[Header("Additional boids")]
		public Pattern additionalBoidsPattern;
		public float additionalBoidsCreateDuration;

		[Header("Image effect")]
		public GenericPreset underwaterOn;
		public MaterialExplorer underwaterMaterial;
		public GenericPreset backgroundWater;
		public MaterialExplorer backgroundMaterial;

		[Header("Fish school 1")]
		public FlockPreset fishSchool1Preset;
		public float fishSchool1TransitionDuration;
		public Transform fish1Template;

		[Header("Fish school 2")]
		public FlockPreset fishSchool2Preset;
		public float fishSchool2TransitionDuration;
		public Pattern additionalSchool2Pattern;

		[Header("Birds 1")]
		public FlockPreset birdsFlock1Preset;
		public float birdsFlock1TransitionDuration;
		public Transform birds1Template;
		public GenericPreset skySunset;
		public GenericPreset backgroundSkySunset;
	}
	public P p;

	[Header("Mock")]
	public bool mockPreviousStep = true;
	public Pattern mockPattern;
	public GameObject[] mockDestroy;

	private Coroutine m_AttractionUpDownCoroutine;

	protected void Awake () {

		instance = this;

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

		if ( !mockPreviousStep ) {

			foreach ( GameObject go in mockDestroy )
				DestroyImmediate ( go );

			flock.spawnPattern = mockPattern;

		}

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

		switch ( step ) {
			case Step.Attraction: {

					if(OrbitController.Instance != null ) {
						OrbitController.Instance.Apply ( p.camView );
					}

					yield return null;
					break;
				}

			case Step.AttractionUpDown: {

					m_AttractionUpDownCoroutine = StartCoroutine ( AttractionUpDownCoroutine () );

					step = Step.Idle;
					parallel = Parallel.LerpToSeparation;
					break;

				}

			case Step.SettleCohesion: {

					Tween.DestroyAll ( flock.originTarget.gameObject );
					//Scene_Camera.Instance.SmoothLookAt ( flock.originTarget, 2f, false );
					flock.TransitionToPreset ( p.fishSchool1Preset, p.fishSchool1TransitionDuration );
					ConvertBoidsToTemplate ( p.fish1Template, p.fishSchool1TransitionDuration );

					float transitionDuration = p.fishSchool1TransitionDuration * .5f;
					IEnumerator transition = GenericPresetsUtil.TransitionFromToCoroutine(
						null, p.underwaterOn, p.underwaterMaterial,
						transitionDuration,
						null, null
					);

					StartCoroutine ( transition );

					IEnumerator transitionBkg = GenericPresetsUtil.TransitionFromToCoroutine(
						null, p.backgroundWater, p.backgroundMaterial,
						transitionDuration,
						null, null
					);

					StartCoroutine ( transitionBkg );

					step = Step.Idle;
					parallel = Parallel.Idle;
					break;
				}

			case Step.School2: {

					if( m_AttractionUpDownCoroutine != null)
						StopCoroutine ( m_AttractionUpDownCoroutine );

					flock.TransitionToPreset ( p.fishSchool2Preset, p.fishSchool2TransitionDuration );
					flock.CreateBoidsOverDuration ( p.additionalSchool2Pattern, p.fishSchool2TransitionDuration );
					Infinity.Apply ( flock.originTarget.gameObject, 10f, 6f, .01f );

					step = Step.Idle;
					break;
				}

			case Step.Birds: {

					Infinity.Remove ( flock.originTarget.gameObject );

					StartCoroutine ( flock.RandomOriginFromBoidsCoroutine ( 1f ) );
					OrbitController.Instance.orbitTarget = flock.originTarget;
					OrbitController.Instance.rotationalDamping *= .2f;

					flock.TransitionToPreset ( p.birdsFlock1Preset, p.birdsFlock1TransitionDuration );
					ConvertBoidsToTemplate ( p.birds1Template, p.birdsFlock1TransitionDuration );
					flock.CreateBoidsOverDuration ( p.additionalSchool2Pattern, p.birdsFlock1TransitionDuration );

					float transitionDuration = p.birdsFlock1TransitionDuration * .5f;
					IEnumerator transition = GenericPresetsUtil.TransitionFromToCoroutine(
						null, p.skySunset, p.underwaterMaterial,
						transitionDuration,
						null, null
					);

					StartCoroutine ( transition );

					IEnumerator transitionBkg = GenericPresetsUtil.TransitionFromToCoroutine(
						null, p.backgroundSkySunset, p.backgroundMaterial,
						transitionDuration,
						null, null
					);

					StartCoroutine ( transitionBkg );

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

		int numBoids = flock.boids.Count;
		float delay = duration / numBoids;
		bool templateUpdated = false;

		int countNulls = 0;
		for(int i = 0; i < numBoids; i++ ) {

			Boid boid = flock.boids[i];

			if ( boid != null ) {

				//SwitchMeshView.Apply ( boid.GetComponentInChildren<MeshFilter>().gameObject, template.gameObject );
				GameObject original = boid.GetComponentInChildren<MeshFilter>().gameObject;
				Instantiate ( template, original.transform.parent );
				Destroy ( original );
				Scene_Particle.AppearZoom ( boid.gameObject, 0f );

				if( templateUpdated == false ) {

					flock.template = boid;

				}

				yield return new WaitForSeconds ( delay );

			}
			else {
				countNulls++;
			}

		}

		Debug.LogFormat ( "Null boids = {0}", countNulls );
		yield return null;

	}

	private IEnumerator ParallelExecution () {

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

	private IEnumerator AttractionUpDownCoroutine () {
		if ( OrbitController.Instance != null )
			OrbitController.Instance.orbitTarget = flock.originTarget;

		Vector3 origin = flock.originTarget.position;
		float amplitude = 0f;

		while ( true ) {
			amplitude = AttractionUpDown ( origin, amplitude );

			yield return new WaitForFixedUpdate ();

		}
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
