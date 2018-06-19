using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCam : MonoBehaviour {

	[System.Serializable]
	public class CameraJSON {

		public Vector3 position, euler;

		internal bool isRead = true;

	}

	public FirebaseManager firebase;

	public bool ready = false;
	public bool isController = false;
	public float updateInterval = .5f;
	public CameraJSON cameraValues = new CameraJSON();

	protected void Start () {

		firebase = FirebaseManager.Instance;
		firebase.ExecuteOnInitialisation ( OnFirebaseReady );

	}

	private void OnFirebaseReady () {

		Debug.Log ( "Firebase ready" );

		ready = true;

		if ( isController ) {

			StartCoroutine ( UpdateValuesCoroutine () );

		}
		else {

			firebase.Listen ( "admin/camera", GetCameraValues );

		}

	}

	private void GetCameraValues ( object value ) {

		JsonUtility.FromJsonOverwrite ( ( string ) value, cameraValues );
		cameraValues.isRead = false;

	}

	private IEnumerator UpdateValuesCoroutine () {

		while ( updateInterval > .001f ) {

			cameraValues.position = transform.position;
			cameraValues.euler = transform.eulerAngles;
			firebase.SetValueAsync ( "admin/camera", JsonUtility.ToJson ( cameraValues ) );

			yield return new WaitForSeconds ( updateInterval );

		}

	}

	protected void Update () {

		if ( cameraValues.isRead == false ) {

			transform.position = cameraValues.position;
			transform.eulerAngles = cameraValues.euler;
			cameraValues.isRead = true;

		}

	}

}
