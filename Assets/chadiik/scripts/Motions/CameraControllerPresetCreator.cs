using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CameraControllerPresetCreator : MonoBehaviour {

	public OrbitController controller;
	public CameraControllerPreset preset;

	public bool isEnabled = true;
	public bool updatePreset = false;

	[Header("Editor save")]
	public string presetName;
	public bool savePreset = false;

	protected void Update () {

		if ( isEnabled && preset != null ) {
			controller.TargetPosition = preset.position;
			controller.TargetRotation = Quaternion.Euler ( preset.eulerAngles );
		}

		if ( updatePreset ) {
			updatePreset = false;

			preset.position = transform.position;
			preset.eulerAngles = transform.rotation.eulerAngles;

#if UNITY_EDITOR
			EditorUtility.SetDirty ( preset );
#endif
		}

#if UNITY_EDITOR
		if ( isEnabled && Application.isEditor && !Application.isPlaying && controller != null ) {
			controller.EditModeUpdate ();
		}
#endif

	}

}
