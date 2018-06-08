using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MandelbrotPathCreator : MonoBehaviour {

	public MandlebrotExplorer explorer;

	public string savePath = "Assets/";
	public MandelbrotPath pathPreset;
	public string presetName;

	private void CreatePreset () {

		pathPreset = ScriptableObject.CreateInstance<MandelbrotPath> ();

	}

	private void SavePreset () {

		string name = string.IsNullOrEmpty ( presetName ) ? "MBPath" + Mathf.Floor ( UnityEngine.Random.value * 999 ).ToString () : presetName;
		string path = savePath + "/" + name + ".asset";
		AssetDatabase.CreateAsset ( pathPreset, path );

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

			if ( Input.GetKeyDown ( KeyCode.S ) ) {

				SavePreset ();

			}

		}

	}
	
}
