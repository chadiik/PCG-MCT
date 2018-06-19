using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireLink : MonoBehaviour {

	public class PresetProperties {

		public List<GenericPreset.Property> p;
		internal string json;

		public bool NeedsWrite ( out string json ) {

			json = JsonUtility.ToJson ( this );
			if ( this.json != json ) {

				this.json = json;
				return true;

			}

			return false;

		}

		public bool NeedsRead ( string json ) {

			if ( this.json != json ) {

				this.json = json;
				JsonUtility.FromJsonOverwrite ( json, this );
				return true;

			}

			return false;
		}

	}

	public enum AccessType { Write, Read, WriteIfAdmin, ReadIfAdmin };

	public FirebaseManager firebase;
	public string databasePath;
	public float updateInterval = .5f;
	internal bool ready = false;

	[Space]

	public AccessType accessType = AccessType.Read;
	public bool writeValuesOnLoad = false;

	[Space]

	public UnityEngine.Object target;
	public string propertiesCSV = "float, int, vector3, color";
	public GenericPreset values;
	internal PresetProperties presetProperties;
	internal bool needsUpdate = false;

	protected void Start () {

		values = GenericPreset.Create ( target, GenericPresetsUtil.GetPropertyNames ( propertiesCSV ) );
		presetProperties = new PresetProperties { p = values.properties };

		firebase = FirebaseManager.Instance;

		if ( firebase == null ) {
			firebase = gameObject.AddComponent<FirebaseManager> ();
			FirebaseManager.IsAdmin = true;
			firebase.id = "default";
		}

		if(firebase != null)
			firebase.ExecuteOnInitialisation ( OnFirebaseReady );

	}

	protected void Update () {

		if ( needsUpdate ) {

			needsUpdate = false;
			values.Apply ( target );

		}

	}

	private void OnFirebaseReady () {

		ready = true;

		switch ( accessType ) {
			case AccessType.WriteIfAdmin:
				accessType = FirebaseManager.IsAdmin ? AccessType.Write : AccessType.Read;
				break;
			case AccessType.ReadIfAdmin:
				accessType = FirebaseManager.IsAdmin ? AccessType.Read : AccessType.Write;
				break;
		}

		firebase.VisualLog ( string.Format ( "AccesType: {0}", accessType.ToString () ) );

		if ( accessType == AccessType.Write) {

			StartCoroutine ( UpdateValuesCoroutine () );

		}
		else {

			if ( writeValuesOnLoad ) {

				UpdateValues ( () => {
					StartListening ();
				} );

			}
			else {

				StartListening ();

			}

		}

	}

	private void StartListening () {

		firebase.Listen ( databasePath, ( object value ) => {

			if ( presetProperties.NeedsRead ( ( string ) value ) ) {

				values.UpdateProperties ( presetProperties.p );
				needsUpdate = true;

			}

		} );

	}

	private IEnumerator UpdateValuesCoroutine () {

		while ( true ) {

			UpdateValues ();

			if ( updateInterval > Time.fixedDeltaTime )
				yield return new WaitForSeconds ( updateInterval );
			else
				yield return new WaitForFixedUpdate ();

		}

	}

	private void UpdateValues ( UnityAction onCompleted = null ) {

		values.UpdateProperties ( target );

		Debug.LogFormat ( "Updating properties: {0}", values.properties.ArrayToString () );

		string json;
		if ( presetProperties.NeedsWrite ( out json ) ) {
			if ( onCompleted == null )
				firebase.SetValueAsync ( databasePath, json );
			else
				firebase.SetValueAsync ( databasePath, json ).ContinueWith (
					task => {
						if ( task.IsCompleted ) {
							onCompleted.Invoke ();
						}
					}
				);
		}

	}
}
