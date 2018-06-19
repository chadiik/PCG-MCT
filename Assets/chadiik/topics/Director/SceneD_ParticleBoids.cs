using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SceneStates {

	public static void PushStates( string type, string current, string[] states, FirebaseManager.ProcessValue onChange ) {

		string currentPath = "scene/" + (type == "primary" ? "currentPrimary" : "currentParallel");
		string statesPath = "scene/" + (type == "primary" ? "primaries" : "parallels");

		FirebaseManager.Instance.SetValueAsync ( currentPath, current ).ContinueWith(
			task => {
				if ( task.IsFaulted ) {
					Director.Instance.debugger.Console.LogLine ( "CurrentPrimary not set!" );
				}
				else if ( task.IsCompleted ) {
					FirebaseManager.Instance.SetValueAsync ( statesPath, states );
					FirebaseManager.Instance.Listen ( currentPath, onChange );
				}
			}
		);
	}

}

public class SceneD_ParticleBoids : SceneD {

	internal Queue<UnityAction> queuedActions = new Queue<UnityAction>();

	protected void Start () {

		StartCoroutine ( Init () );

	}

	protected void FixedUpdate () {

		if ( queuedActions.Count > 0 )
			queuedActions.Dequeue ().Invoke ();

		if ( Scene_Particle.instance.step == Scene_Particle.Step.Boids ) {
			queuedActions.Enqueue ( () => {
				SwitchToBoids ();
			} );

			Scene_Particle.instance.step = Scene_Particle.Step.Idle;
		}

	}

	private IEnumerator Init () {

		while ( Scene_Particle.instance == null )
			yield return new WaitForEndOfFrame ();

		IEnumerable<Scene_Particle.Step> primaries = Enum.GetValues( typeof ( Scene_Particle.Step ) ).Cast<Scene_Particle.Step> ();
		SceneStates.PushStates ( "primary",
			Scene_Particle.instance.step.ToString (),
			primaries.Select ( p => p.ToString () ).ToArray (), 
			OnParticlesPrimaryChange
		);

		OrbitController.Instance.enabled = false;
	}

	private void OnParticlesPrimaryChange ( object value ) {

		string currentPrimary = (string)value;
		Debug.LogFormat ( "currentPrimary changed: {0}", currentPrimary );
		Scene_Particle.instance.step = ( Scene_Particle.Step ) Enum.Parse ( typeof ( Scene_Particle.Step ), currentPrimary );

	}

	private void SwitchToBoids () {

		FirebaseManager.Instance.StopListen ( OnParticlesPrimaryChange );

		Scene_Boids.instance.gameObject.SetActive ( true );

		OrbitController.Instance.enabled = true;

		IEnumerable<Scene_Boids.Step> primaries = Enum.GetValues( typeof ( Scene_Boids.Step ) ).Cast<Scene_Boids.Step> ();
		SceneStates.PushStates ( "primary",
			Scene_Boids.instance.step.ToString (),
			primaries.Select ( p => p.ToString () ).ToArray (),
			OnBoidsPrimaryChange
		);
	}

	private void OnBoidsPrimaryChange ( object value ) {

		string currentPrimary = (string)value;
		Debug.LogFormat ( "currentPrimary changed: {0}", currentPrimary );
		Scene_Boids.instance.step = ( Scene_Boids.Step ) Enum.Parse ( typeof ( Scene_Boids.Step ), currentPrimary );

	}
}
