using pcg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Attractors : MonoBehaviour {

	public bool mainScenario = true;
	public Scene_Particle mainParticle;
	public Transform orbitTemplate;

	public enum Step { Idle, ConvertMainToAttractor, ConvertToBoids };
	public Step step;

	public Rand rand;
	public Direction direction;

	public Scene_Boids boidsObject;

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

		switch ( step ) {

			case Step.ConvertMainToAttractor: {

					InstantiateOrbit ( transform.position );

					step = Step.Idle;
					break;
				}

			case Step.ConvertToBoids: {

					ConvertToBoids ();

					step = Step.Idle;
					break;

				}

			default: {

					break;
				}

		}

		yield return null;
	}

	private void ConvertToBoids () {

		boidsObject.gameObject.SetActive ( true );
		boidsObject.flock.originTarget = ( new GameObject ( "Flock target" ) ).transform;
		boidsObject.flock.originTarget.position = transform.position;

		Scene_Particle[] sceneParticles = GameObject.FindObjectsOfType<Scene_Particle>();

		foreach ( Scene_Particle p in sceneParticles ) {

			Boid boid = boidsObject.flock.InstantiateBoid ( p.transform.position );
			boid.CurrentHeading = p.direction.vector;

			Rigidbody rb = p.GetComponent<Rigidbody>();
			if ( rb != null ) boid.CurrentHeading = rb.velocity;

			/*
			AffectedParticleRB ap = p.GetComponent<AffectedParticleRB>();
			if ( ap != null ) Destroy ( ap );

			Rigidbody rb = p.GetComponent<Rigidbody>();
			if ( rb != null ) Destroy ( ap );
			*/

			Destroy ( p.gameObject );

		}

	}

	private void InstantiateOrbit ( Vector3 position ) {

		Transform orbitPoint = Instantiate ( orbitTemplate );
		orbitPoint.transform.position = position;

		ParticleAffectorRB[] affectors = orbitPoint.GetComponentsInChildren<ParticleAffectorRB> ();

		AffectedParticleRB.commonAffectors.AddRange ( affectors );

	}

	
}
