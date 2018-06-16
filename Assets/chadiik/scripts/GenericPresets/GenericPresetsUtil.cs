using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenericPresetsUtil : MonoBehaviour {

	public MonoBehaviour target;
	public string propertiesCSV = "a, b, v3, color";

	[Space]
	[Header("Preset")]
	public GenericPreset[] lerpPresets;
	private int m_LerpPresetIndex = 0;
	private GenericPreset m_LerpPresetStore;
	public GenericPreset preset;
	public string newPresetName;
	public string presetsPath = "Assets/";
	public bool savePreset = false;

	protected void Start () {

	}

	protected void Update () {

		if ( savePreset ) {

			savePreset = false;
			CreatePreset ();

		}

		if(preset != null ) {

			if ( target is MaterialExplorer )
				preset.ApplyMaterial ( ( ( MaterialExplorer ) target ).material );
			else
				preset.Apply ( target );

			preset = null;

		}

		int numPresets = lerpPresets.Length;

		if ( numPresets > 1 ) {

			LerpPresets ();

		}

	}

	private void LerpPresets () {

		if ( m_LerpPresetStore == null )
			m_LerpPresetStore = ScriptableObject.CreateInstance<GenericPreset> ();

		int numPresets = lerpPresets.Length;

		if ( m_LerpPresetIndex >= numPresets )
			m_LerpPresetIndex = 0;

		int nextIndex = m_LerpPresetIndex + 1 < numPresets ? m_LerpPresetIndex + 1: 0;

		if ( lerpPresets [ m_LerpPresetIndex ] != null && lerpPresets [ nextIndex ] != null ) {

			GenericPreset.Lerp ( lerpPresets [ m_LerpPresetIndex ], lerpPresets [ nextIndex ], Mathf.Sin ( Time.time * .2f ) * .5f + .5f, m_LerpPresetStore );

			if ( target is MaterialExplorer )
				m_LerpPresetStore.ApplyMaterial ( ( ( MaterialExplorer ) target ).material );
			else
				m_LerpPresetStore.Apply ( target );

			m_LerpPresetIndex++;

		}

	}

	private string[] GetPropertyNames () {

		string[] properties = propertiesCSV.Split(new[] {','});
		for ( int i = 0, numProperties = properties.Length; i < numProperties; i++ ) {
			properties [ i ] = properties [ i ].Trim ();
		}

		return properties;

	}

	private void CreatePreset () {

		GenericPreset newPreset = target is MaterialExplorer ?
			GenericPreset.Create ( ((MaterialExplorer) target).properties ) :
			GenericPreset.Create ( target, GetPropertyNames () );

		string path = presetsPath + "/Presets";
		if ( !AssetDatabase.IsValidFolder ( path ) )
			AssetDatabase.CreateFolder ( presetsPath, "Presets" );

		string newName = string.IsNullOrEmpty( newPresetName ) ? "Generic" + DateTime.Now.Ticks : newPresetName;
		string fullPath = path + "/" + newName + ".asset";
		GenericPreset existing = AssetDatabase.LoadAssetAtPath<GenericPreset> ( fullPath );

		if(existing != null)
			fullPath = path + "/" + newName + "_" + DateTime.Now.Ticks + ".asset";

		AssetDatabase.CreateAsset ( newPreset, fullPath );

	}

}
