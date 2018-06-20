using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SceneD_CA : SceneD {

	internal Queue<UnityAction> queuedActions = new Queue<UnityAction>();

	protected void Start () {

		StartCoroutine ( Init () );

	}

	protected void FixedUpdate () {

		if ( queuedActions.Count > 0 )
			queuedActions.Dequeue ().Invoke ();

	}

	private IEnumerator Init () {

		while ( SceneCA.instance == null )
			yield return new WaitForEndOfFrame ();

		IEnumerable<SceneCA.Step> primaries = Enum.GetValues( typeof ( SceneCA.Step ) ).Cast<SceneCA.Step> ();
		SceneStates.PushStates ( "primary",
			SceneCA.instance.step.ToString (),
			primaries.Select ( p => p.ToString () ).ToArray (),
			OnPrimaryChange
		);

		//OrbitController.Instance.enabled = false;
	}

	private void OnPrimaryChange ( object value ) {

		string currentPrimary = (string)value;
		Debug.LogFormat ( "currentPrimary changed: {0}", currentPrimary );
		SceneCA.instance.step = ( SceneCA.Step ) Enum.Parse ( typeof ( SceneCA.Step ), currentPrimary );

	}

}
