using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PV = PropValueKey;

public class ReactionDiffusionMaterial : MonoBehaviour {

	[SerializeField]
	RDPreset _parameters;

	public RDPreset Parameters {
		set {

			m_PresetUnique = true;
			_parameters = value;

		}

		get {

			return _parameters;

		}
	}

	public int updateRate = 1;
	public Material material;
	public CustomRenderTexture texture;
	public bool needsUpdate = true;

	[Header ( "Region" )]
	public bool updateRegion = false;
	public bool resetRegion = false;
	private RDPreset m_RegionBackup;

	[Header ( "Randomizer" )]
	public RDPresetRandomizer randomizer;
	public bool randomize = false;
	private bool m_PresetUnique = false;

	public void Randomize () {

		if ( m_PresetUnique == false ) {

			Parameters = Instantiate<RDPreset> ( _parameters );

		}

		if ( randomizer == null ) randomizer = ScriptableObject.CreateInstance<RDPresetRandomizer> ();

		randomizer.Apply ( _parameters );

		texture.Initialize ();

		needsUpdate = true;

	}

	private void UpdateValues () {

		material.SetVector ( "_Convolution", new Vector4 ( _parameters.convCell, _parameters.convAdj, _parameters.convDiag ) );

		material.SetFloat ( "_A", _parameters.aRate );
		material.SetFloat ( "_B", _parameters.bRate );
		material.SetFloat ( "_Feed", _parameters.feedRate );
		material.SetFloat ( "_Kill", _parameters.killRate );

		material.SetFloat ( "_DT", _parameters.deltaTime );

	}

	protected void Start () {

		texture.Initialize ();

		float feedKillRadius = .005f;
		float abRatesRadius = .00001f;

		if ( randomizer == null ) {

			randomizer = ScriptableObject.CreateInstance<RDPresetRandomizer> ();
			//randomizer.InitAroundBase ( Parameters, regionRadius );
			randomizer.InitAroundBase ( Parameters, new PV[]{
				
				new PV("convCell", 0f),
				new PV("convAdj", 0f),
				new PV("convDiag", 0f),

				new PV("feedRate", feedKillRadius),
				new PV("killRate", feedKillRadius),
				new PV("aRate", abRatesRadius),
				new PV("bRate", abRatesRadius),

				new PV("deltaTime", 0f)
				
			} );

		}

		feedKillData = new DynamicData ( string.Format ( "regionRadius={0},regionRadiusBackup={0}!", feedKillRadius ) );

		ABRatesData = new DynamicData ( string.Format ( "regionRadius={0},regionRadiusBackup={0}!", feedKillRadius ) );

		m_RegionBackup = Parameters.Copy ();

	}

	private Transform feedKillValue, ABRatesValue;
	public DynamicData feedKillData, ABRatesData;
	protected void Update () {

		if ( resetRegion ) {

			m_RegionBackup.Copy ( _parameters );
			feedKillData.Set ( "regionRadius", feedKillData.Get ( "regionRadiusBackup" ).value );
			ABRatesData.Set ( "regionRadius", ABRatesData.Get ( "regionRadiusBackup" ).value );

			resetRegion = false;
			updateRegion = true;

		}

		if ( updateRegion ) {

			if ( m_PresetUnique == false ) {

				Parameters = Instantiate<RDPreset> ( _parameters );

			}

			float feedKillRadius = feedKillData.Get ( "regionRadius" ).value;
			feedKillData.Set ( "regionRadiusBackup", feedKillRadius );

			float ABRatesRadius = ABRatesData.Get ( "regionRadius" ).value;
			ABRatesData.Set ( "regionRadiusBackup", ABRatesRadius );

			randomizer.InitAroundBase ( Parameters, new PV[]{
				
				new PV("feedRate", feedKillRadius),
				new PV("killRate", feedKillRadius),
				new PV("aRate", ABRatesRadius),
				new PV("bRate", ABRatesRadius)
				
			});

			updateRegion = false;

			Vector2 feedKillUV = new Vector2 ( randomizer.Unlerp ( "feedRate", _parameters.feedRate ), randomizer.Unlerp ( "killRate", _parameters.killRate ) );
			UVParameters ( feedKillUV, 4 );

			Vector2 ABUV = new Vector2 ( randomizer.Unlerp ( "aRate", _parameters.aRate ), randomizer.Unlerp ( "bRate", _parameters.bRate ) );
			UVParameters ( ABUV, 2 );

			Parameters.Copy ( m_RegionBackup );

		}

		if ( feedKillValue == null ) {

			feedKillValue = GameObject.CreatePrimitive ( PrimitiveType.Sphere ).transform;
			feedKillValue.name = "feedKill";
			feedKillValue.localScale = Vector3.one * .01f;
			feedKillValue.GetComponent<Renderer> ().material.color = Color.blue;

			Vector2 feedKillUV = new Vector2 ( randomizer.Unlerp ( "feedRate", _parameters.feedRate ), randomizer.Unlerp ( "killRate", _parameters.killRate ) );
			Debug.Log ( feedKillUV );
			feedKillValue.position = GetComponent<MeshFilter> ().UVToWorldPos ( feedKillUV, new int[] { 4, 5 } );




			ABRatesValue = GameObject.CreatePrimitive ( PrimitiveType.Sphere ).transform;
			ABRatesValue.name = "ABRates";
			ABRatesValue.localScale = Vector3.one * .01f;
			ABRatesValue.GetComponent<Renderer> ().material.color = Color.green;

			Vector2 ABUV = new Vector2 ( randomizer.Unlerp ( "aRate", _parameters.aRate ), randomizer.Unlerp ( "bRate", _parameters.bRate ) );
			ABRatesValue.position = GetComponent<MeshFilter> ().UVToWorldPos ( ABUV, new int[] { 2, 3 } );

		}

		if ( Input.GetMouseButton ( 0 ) ) {

			if ( m_PresetUnique == false ) {

				Parameters = Instantiate<RDPreset> ( _parameters );

			}

			Ray ray = Camera.main.ScreenPointToRay ( Input.mousePosition );

			RaycastHit hit;

			if ( Physics.Raycast ( ray, out hit ) && hit.transform == transform ) {

				Vector2 uv = hit.textureCoord;
				int tIndex = hit.triangleIndex;

				Debug.LogFormat ( "Triangle index = {0}", tIndex );

				UVParameters ( uv, tIndex );

			}



		}

	}

	private void UVParameters ( Vector2 uv, int tIndex ) {
		bool feedKill = tIndex == 4 || tIndex == 5;
		bool ABRates = tIndex == 2 || tIndex == 3;
		bool something = tIndex == 8 || tIndex == 9;

		if ( feedKill ) {

			Vector3 point = GetComponent<MeshFilter> ().UVToWorldPos ( uv, new int[] { 4, 5 } );
			feedKillValue.position = point;

			_parameters.feedRate = randomizer.Lerp ( "feedRate", uv.x );
			_parameters.killRate = randomizer.Lerp ( "killRate", uv.y );

			needsUpdate = true;

		}
		else if ( ABRates ) {

			Vector3 point = GetComponent<MeshFilter> ().UVToWorldPos ( uv, new int[] { 2, 3 } );
			ABRatesValue.position = point;

			_parameters.aRate = randomizer.Lerp ( "aRate", uv.x );
			_parameters.bRate = randomizer.Lerp ( "bRate", uv.y );

			needsUpdate = true;

		}
	}

	protected void FixedUpdate () {

#if UNITY_EDITOR
		RDP_FixedUpdate ();
#endif

		if ( randomize ) {

			Randomize ();

			randomize = false;

		}

		if ( Parameters.hasChanged ) {

			texture.Initialize ();
			needsUpdate = true;

		}

		if ( needsUpdate ) {

			UpdateValues ();
			needsUpdate = false;

		}

		if ( updateRate > 0 ) {

			texture.Update ( updateRate );

		}

	}

	/*
	[UnityEditor.MenuItem ( "pcg/Reaction Diffusion/Save preset" )]
	static void SavePreset () {

		string name = "Preset";
		UnityEditor.AssetDatabase.CreateAsset ( ScriptableObject.CreateInstance ( typeof ( RDPreset ) ), "Assets/chadiik/topics/reactionDiffusion/RDPresets/" + name + ".asset" );

	}
	 * */

#if UNITY_EDITOR

	[Header ( "Preset" )]
	public string RDPName = "auto";
	public bool RDPSave = false;

	private void RDP_FixedUpdate () {

		if ( RDPSave ) {

			string name = RDPName == "auto" ? _parameters.ID : RDPName;
			UnityEditor.AssetDatabase.CreateAsset ( _parameters, "Assets/chadiik/topics/reactionDiffusion/RDPresets/" + name + ".asset" );

			RDPSave = false;
			m_PresetUnique = false;

		}

	}

#endif
}
