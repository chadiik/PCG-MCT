﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MandelbrotPathCreator : MonoBehaviour {

	public MandlebrotExplorer explorer;

	public string workingPath = "Assets/";
	public MandelbrotPath pathPreset;
	public string presetName;

	private void CreatePreset () {

		pathPreset = ScriptableObject.CreateInstance<MandelbrotPath> ();

	}

	private void SavePreset () {

#if UNITY_EDITOR
		string savePath = workingPath + "/Presets";

		if ( AssetDatabase.IsValidFolder ( savePath ) == false )
			AssetDatabase.CreateFolder ( workingPath, "Presets" );

		string name = string.IsNullOrEmpty ( presetName ) ? "MBPath" + Mathf.Floor ( UnityEngine.Random.value * 999 ).ToString () : presetName;
		string path = savePath + "/" + name + ".asset";
		AssetDatabase.CreateAsset ( pathPreset, path );
#endif

	}

	private void AddNode () {

		pathPreset.AddNode ( explorer.x, explorer.y, explorer.scale, explorer.rotation );

	}

	private void RemoveLastNode () {

		pathPreset.RemoveLastNode ();

	}

	protected void Start () {

		CreatePreset ();

	}

	protected void Update () {

		if ( Input.GetKeyDown ( KeyCode.Space ) ) {

			AddNode ();

		}
		else if ( Input.GetKeyDown ( KeyCode.Backspace ) ) {

			RemoveLastNode ();

		}
		else if ( Input.GetKeyDown ( KeyCode.KeypadEnter ) ) {

			CreatePreset ();

		}

		if ( Input.GetKey ( KeyCode.LeftControl ) ) {

			if ( Input.GetKeyDown ( KeyCode.LeftAlt ) ) {

				SavePreset ();

			}

		}

	}
	
}
