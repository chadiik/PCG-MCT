using pcg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Attractors : MonoBehaviour {

	public bool mainScenario = true;
	public Scene_Particle mainParticle;
	public Transform orbitTemplate;

	public enum Step { Idle, ConvertMainToAttractor };
	public Step step;

	public Rand rand;
	public Direction direction;

	protected void Start () {

		if ( rand == null )
			rand = gameObject.AddComponent<Rand> ();

		if ( direction == null )
			direction = new Direction ();

		direction.Rand = rand;

		if ( mainScenario ) {

			StartCoroutine ( ScenarioCoroutine () );

		}

	}

	private IEnumerator ScenarioCoroutine () {

		while ( isActiveAndEnabled )
			yield return ScenarioExecution ();

	}

	private IEnumerator ScenarioExecution () {

		Vector3 newParticleDirection;
		float time, t;

		switch ( step ) {

			case Step.ConvertMainToAttractor: {

					InstantiateOrbit ( transform.position );

					step = Step.Idle;
					break;
				}

			default: {

					break;
				}

		}

		yield return null;
	}

	private void InstantiateOrbit ( Vector3 position ) {

		Transform orbitPoint = Instantiate ( orbitTemplate );
		orbitPoint.transform.position = position;

		ParticleAffectorRB[] affectors = orbitPoint.GetComponentsInChildren<ParticleAffectorRB> ();

		AffectedParticleRB.commonAffectors.AddRange ( affectors );

	}

	
}
