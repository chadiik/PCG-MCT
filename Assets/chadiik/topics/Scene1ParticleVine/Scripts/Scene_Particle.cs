using pcg;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Uween;

public class Scene_Particle : MonoBehaviour {

	public static Scene_Particle instance;

	public const int MAX_PARTICLES = 60;
	public static int particlesCount = 0;
	public static float particlesCreationInterval = .1f;
	public static float lastCreatedParticleTime = 0f;

	public bool mainScenario = false;

	[System.Serializable]
	public class P {
		internal float particleSpeed = 0f;
		public float brownianCycleDuration = .1f;
		public float particleBrownianSpeedStart = 0f, particleBrownianSpeedEnd = .1f, particleUpToSpeedDuration = 5f;
		public float particleConeStart = 0f, particleConeEnd;

		[Header("Camera")]
		public Camera camera;
		public float cameraFollowSmooth = .1f;
		public Vector3 cameraFollowTarget;
		public Vector3 cameraFollowOffset;

		[Header("Vine tip")]
		public float lifeDuration = float.MaxValue;
		internal float life;
		public float vineShapeSize = 1f;

		[Header("Vine branching")]
		public float automatedBranchingRate = 0f;
		public int maxLevel = 4;
		public float minBranchLifeDuration = 1f;
		public float maxBranchLifeDuration = 10f;
		public float lifeDurationMultiplier = .7f;
		public float levelShapeSizeMultiplier = .7f;
	}
	public P p;

	public enum Step { Idle, AppearZoom, UpToSpeed, Travel, AffectedParticleRB, BeginVineTrail, AutomateBranching, BackEraseVines, Boids };
	public Step step = Step.AppearZoom;

	public Rand rand;
	public Direction direction;

	internal AffectedParticleRB affectedParticle;
	[Header("Vine")]
	public Transform vineTemplate;
	public Scene_Vine vine;
	public Transform branchingParticlesTemplate;
	public int level = 0;

	public List<Scene_Particle> branches;
	public bool automatedBranching = false;
	public bool addBranch = false;
	//internal 

	public static UnityAction EMPTY = () => { };
	public UnityAction bloom = EMPTY;

	protected void Awake () {
		if(instance == null)
			instance = this;
	}

	protected void Start() {

		particlesCount++;
		lastCreatedParticleTime = Time.time;

		if ( rand == null )
			rand = gameObject.AddComponent<Rand> ();

		if ( direction == null )
			direction = new Direction ();

		direction.Rand = rand;

		affectedParticle = gameObject.GetComponent<AffectedParticleRB> ();
		affectedParticle.enabled = false;

		if ( mainScenario ) {

			StartCoroutine ( ScenarioCoroutine () );

		}
		else {

			StartCoroutine ( BranchingCoroutine () );

		}

		affectedParticle.motion = DirectedAffectedBrownian;

	}

	protected void FixedUpdate () {

		AutomatedBranching ();

		if ( addBranch ) {
			addBranch = false;

			Branch ();

		}

		UpdateLife ();

	}


	public void AddAffectors ( ParticleAffectorRB [] affectors ) {

		foreach ( ParticleAffectorRB affector in affectors ) {

			affectedParticle.affectors.Add ( affector );

		}

		foreach ( Scene_Particle particle in branches ) {

			particle.AddAffectors ( affectors );

		}

	}

	public void DirectedAffectedBrownian ( object value ) {

		AffectedParticleRB particle = affectedParticle;// value as AffectedParticleRB;

		Vector3 newDirection = direction.GetRandom ();
		direction.vector = newDirection;
		Vector3 mForce = newDirection * particle.speed * p.particleSpeed;
		particle.RB.AddForce ( mForce, ForceMode.Force );

		Vector3 affectorsPull = AffectedParticleRB.AggregateAffectorsInfCurve ( particle.transform.position, particle.affectors ) * particle.effectStrength;
		affectorsPull += AffectedParticleRB.AggregateAffectorsInfCurve ( particle.transform.position, AffectedParticleRB.commonAffectors ) * particle.effectStrength;
		particle.RB.AddForce ( affectorsPull, ForceMode.Force );

	}

	public IEnumerator ScheduledErase( IEnumerable<UnityEngine.Object> list, UnityAction onComplete = null ) {

		Debug.LogFormat ( "Erasing: {0}", list.ArrayToString () );
		float delay = Mathf.Min(.1f, 4f / (float)list.Count());

		foreach(UnityEngine.Object item in list) {

			yield return new WaitForSeconds ( delay );
			Destroy ( item );

		}

		if ( onComplete != null )
			onComplete.Invoke ();

	}

	public void BackEraseVines ( List<UnityEngine.Object> vineViews = null ) {

		if ( vineViews == null )
			vineViews = new List<UnityEngine.Object> ();

		foreach ( Scene_Particle particle in branches ) {

			if(particle != null)
				particle.BackEraseVines ( vineViews );

		}

		//Debug.LogFormat ( "BackEraseVines at level {0}, {1} branches", level, branches.Count );

		if ( vine != null ) {
			List<Transform> branchViews = vine.main.CurrentSplineExtruder.Views;
			//Debug.LogFormat ( "\t{0} views", branchViews.Count );
			for(int i = branchViews.Count; --i >= 0; ) {

				vineViews.Add ( branchViews [ i ].gameObject );

			}

			vineViews.Add ( vine.gameObject );
		}

		//Debug.LogFormat ( "mainScenario: {0}, {1} vineViews", mainScenario, vineViews.Count );
		if ( mainScenario ) {

			//vine.gameObject.SetActive ( false );
			vineViews.Add ( vine.gameObject );
			StartCoroutine ( ScheduledErase ( vineViews, () => {

				Scene_Vine[] vines = GameObject.FindObjectsOfType<Scene_Vine>();
				foreach(Scene_Vine vine in vines ) {

					Destroy ( vine.gameObject );

				}

			} ));

		}

	}

	public void StopVineGrowth () {

		foreach ( Scene_Particle particle in branches ) {

			if ( particle != null )
				particle.StopVineGrowth ();

		}

		automatedBranching = false;
		p.automatedBranchingRate = 0f;

		if ( vine != null ) {
			vine.enabled = false;
		}

	}

	private void AutomatedBranching () {

		if ( automatedBranching ) {

			float time = Time.time;
			if ( time > lastCreatedParticleTime + particlesCreationInterval && particlesCount < MAX_PARTICLES && p.automatedBranchingRate > .0000001f && level < p.maxLevel ) {

				float branchProb = rand.Float ();
				if ( branchProb < p.automatedBranchingRate ) {

					Branch ();

				}

			}

		}

	}

	private IEnumerator ScenarioCoroutine () {

		p.cameraFollowOffset = p.camera.transform.position - transform.position;

		while ( isActiveAndEnabled )
			yield return ScenarioExecution ();

	}

	private IEnumerator ScenarioExecution () {

		Vector3 newParticleDirection;
		float time, t;

		switch ( step ) {

			case Step.AppearZoom: {
					AppearZoom ( gameObject, 1f );
					yield return new WaitForSeconds ( 1f );

					step = Step.Idle;
					break;
				}


			case Step.UpToSpeed: {
					#region get up to speed

					p.particleSpeed = p.particleBrownianSpeedStart;
					direction.coneRadius = p.particleConeStart;
					float duration = p.particleUpToSpeedDuration, start = Time.time;
					while ( p.particleSpeed < p.particleBrownianSpeedEnd ) {

						time = Time.time;
						t = ( time - start ) / duration;
						p.particleSpeed = LF ( p.particleBrownianSpeedStart, p.particleBrownianSpeedEnd, t );
						//direction.coneRadius = LF ( p.particleConeStart, p.particleConeEnd, t );
						direction.coneRadius = LF ( p.particleConeStart, p.particleConeEnd, Mathf.Sin ( time * p.brownianCycleDuration ) * .5f + .5f );

						newParticleDirection = direction.GetRandom ();
						UpdateParticleWith ( newParticleDirection, p.particleSpeed );
						//direction.vector = newParticleDirection;

						p.cameraFollowTarget = transform.position;
						UpdateCameraPosition ();

						yield return new WaitForFixedUpdate ();
					}

					#endregion

					step = Step.Travel;
					break;
				}


			case Step.AffectedParticleRB: {
					affectedParticle.enabled = true;
					Vector3 velocity = direction.vector * p.particleSpeed;
					affectedParticle.GetComponent<Rigidbody> ().AddForce ( velocity * 2f, ForceMode.Impulse );

					//step = Step.Idle;
					step = Step.Idle;
					break;
				}


			case Step.BeginVineTrail: {
					TrailRenderer trail = gameObject.GetComponent<TrailRenderer>();
					trail = null;
					if ( trail != null )
						trail.enabled = false;

					if(vine == null ) {

						Transform vineObject = Instantiate<Transform>( vineTemplate );
						vineObject.transform.position = Vector3.zero;
						vine = vineObject.GetComponent<Scene_Vine> ();

					}

					vine.p.size = p.vineShapeSize;
					vine.mainTarget = transform;
					vine.Begin ();

					step = Step.Idle;
					break;
				}

			case Step.AutomateBranching: {

					automatedBranching = true;

					step = Step.Idle;
					break;
				}

			case Step.Travel: {
					#region travel
					time = Time.time;
					t = time * p.brownianCycleDuration;
					direction.coneRadius = LF ( p.particleConeStart, p.particleConeEnd, Mathf.Sin ( t ) * .5f + .5f );
					newParticleDirection = direction.GetRandom ();
					UpdateParticleWith ( newParticleDirection, p.particleSpeed );
					//direction.vector = newParticleDirectionB;

					p.cameraFollowTarget = transform.position;
					UpdateCameraPosition ();
					#endregion

					yield return new WaitForFixedUpdate ();
					break;
				}

			case Step.BackEraseVines: {
					BackEraseVines ();
					StopVineGrowth ();

					step = Step.Idle;
					break;

				}

			default: {
					p.cameraFollowTarget = transform.position;
					UpdateCameraPosition ();

					yield return new WaitForFixedUpdate ();
					break;
				}
		}

	}

	internal bool branchInit = false;
	public void Branch () {

		if ( branches == null ) {
			branches = new List<Scene_Particle> ();
		}

		if( mainScenario && branchInit == false ) {
			branchInit = true;
			p.cameraFollowOffset *= 4f;
		}

		Transform branchObject = Instantiate<Transform>(branchingParticlesTemplate);
		branchObject.position = transform.position;

		Scene_Particle branchParticle = branchObject.GetComponent<Scene_Particle>();
		branchParticle.branchingParticlesTemplate = branchingParticlesTemplate;
		branchParticle.vineTemplate = vineTemplate;

		branchParticle.level = level + 1;
		branchParticle.p.maxLevel = p.maxLevel;

		Vector3 velocity = affectedParticle.RB.velocity;
		branchParticle.p.particleSpeed = velocity.magnitude / 10f;
		branchParticle.GetComponent<AffectedParticleRB> ().GetComponent<Rigidbody> ().AddForce ( velocity * 2f, ForceMode.Impulse );

		branchParticle.direction.vector = direction.vector;
		branchParticle.direction.coneRadius = direction.coneRadius / p.lifeDurationMultiplier;

		branchParticle.p.minBranchLifeDuration = p.minBranchLifeDuration * p.lifeDurationMultiplier;
		branchParticle.p.maxBranchLifeDuration = p.maxBranchLifeDuration * p.lifeDurationMultiplier;
		branchParticle.p.lifeDurationMultiplier = p.lifeDurationMultiplier;
		branchParticle.p.lifeDuration = rand.Float ( branchParticle.p.minBranchLifeDuration, branchParticle.p.maxBranchLifeDuration );

		branchParticle.transform.localScale = branchParticle.transform.localScale * p.lifeDurationMultiplier;

		branchParticle.branchInit = true;
		branchParticle.p.vineShapeSize = p.vineShapeSize * p.levelShapeSizeMultiplier;

		branches.Add ( branchParticle );

	}

	private IEnumerator BranchingCoroutine () {

		while ( isActiveAndEnabled )
			yield return BranchingExecution ();

	}

	private IEnumerator BranchingExecution () {

		Vector3 newParticleDirection;
		float time, t;

		switch ( step ) {

			case Step.AppearZoom: {
					AppearZoom ( gameObject, 0f );
					yield return new WaitForSeconds ( .2f );

					step = Step.AffectedParticleRB;
					break;
				}


			case Step.AffectedParticleRB: {
					affectedParticle.enabled = true;

					//step = Step.Idle;
					step = Step.BeginVineTrail;
					break;
				}


			case Step.BeginVineTrail: {
					TrailRenderer trail = gameObject.GetComponent<TrailRenderer>();
					trail = null;
					if ( trail != null )
						trail.enabled = false;

					if ( vine == null ) {

						Transform vineObject = Instantiate<Transform>( vineTemplate );
						vineObject.transform.position = Vector3.zero;
						vine = vineObject.GetComponent<Scene_Vine> ();

					}

					vine.p.size = p.vineShapeSize;
					vine.mainTarget = transform;
					vine.Begin ();

					step = Step.Idle;
					break;
				}

			case Step.Travel: {
					#region travel
					time = Time.time;
					t = time * p.brownianCycleDuration;
					direction.coneRadius = LF ( p.particleConeStart, p.particleConeEnd, Mathf.Sin ( t ) * .5f + .5f );
					newParticleDirection = direction.GetRandom ();
					UpdateParticleWith ( newParticleDirection, p.particleSpeed );
					//direction.vector = newParticleDirectionB;

					#endregion

					yield return new WaitForFixedUpdate ();
					break;
				}


			default: {

					yield return new WaitForFixedUpdate ();
					break;
				}
		}

	}

	private void UpdateCameraPosition () {

		Vector3 camPosition = p.camera.transform.position;
		camPosition = Vector3.Lerp ( camPosition, p.cameraFollowTarget + p.cameraFollowOffset, p.cameraFollowSmooth );

		p.camera.transform.position = camPosition;

	}

	public void UpdateParticleWith ( Vector3 direction, float particleSpeed ) {

		Vector3 position = transform.position;
		position += direction * particleSpeed;

		transform.LookAt ( position );
		transform.position = position;

	}

	public void UpdateLife () {

		p.life += Time.fixedDeltaTime;

		if (p.life > p.lifeDuration ) {

			EndLife ();

		}

	}

	private void EndLife () {

		bloom.Invoke ();
		particlesCount--;

		if ( vine != null )
			Destroy ( gameObject );

	}

	/////

	public static float LF (float start, float end, float t ) {
		return start + ( end - start ) * t;
	}

	public static void AppearZoom (GameObject target, float delay) {
		TweenS3.Add ( target, .5f ).From ( Vector3.zero ).EaseOutElasticWith ( 2, .5f ).Delay ( delay );
	}

}
