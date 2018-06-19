using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamRD : ReactionDiffusionMapped, ColorToRates.IRates {

	[Header ( "Webcam" )]
	public Material ratesMaterial;
	public RenderTexture ratesRenderTexture;
	public Material albedoMaterial;

	protected override void Start () {

		m_RDMaterial = texture.material;

		m_RDMaterial.SetVector ( "_Timevar", new Vector4 ( 0, 0, 0, 0 ) );

		//ApplyPreset ();
		CollectAssets ();

		int seed = sortingSeed == 0 ? ( int )System.DateTime.Now.Ticks : sortingSeed;
		Debug.Log ( "Rand.seed = " + seed );
		Rand.Instance.Seed ( seed );
		presetsInfo.Sort ( NSortableList.SortRandom );
		Debug.Log ( presetsInfo );
		ApplyPresets ();

		StartCoroutine ( SetRatesMap () );

	}

	private IEnumerator SetRatesMap () {

		int updateRateValue = updateRate;
		updateRate = 0;

		while ( ratesMaterial.mainTexture == null ) {


			yield return new WaitForEndOfFrame ();

		}

		ratesRenderTexture = ( RenderTexture )ratesMaterial.mainTexture;
		m_RDMaterial.SetTexture ( "_Rates", ratesRenderTexture );

		if ( convolutionMap == null ) convolutionMap = ReactionDiffusionMaterial.StandardConvolutionMap;
		m_RDMaterial.SetTexture ( "_Convolution", convolutionMap );

		if ( displayMaterial == null ) displayMaterial = GetComponent<Renderer> ().sharedMaterial;

		texture.Initialize ();

		updateRate = updateRateValue;

	}

	protected override void ApplyPresets () {
		


	}

	private bool ratesSaved;
	protected override void FixedUpdate () {

		if ( updateRate > 0 ) {

			ratesRenderTexture = ( RenderTexture )ratesMaterial.mainTexture;
			m_RDMaterial.SetTexture ( "_Rates", ratesRenderTexture );

			displayMaterial.SetTexture ( "_AlbedoTex", albedoMaterial.mainTexture );

		}

		base.FixedUpdate ();

	}

	protected override void Update () {

		if ( Input.GetKeyDown ( KeyCode.Space ) ) {

			Debug.Log ( "Time activated" );
			m_TimeActive = true;
			m_Time = 0;

		}

		if ( m_TimeActive ) {

			UpdateTimeValues ();

		}

	}

	private bool m_TimeActive = false;
	private float m_Time = 0;
	private void UpdateTimeValues () {

		m_Time += Time.deltaTime;
		float t = m_Time * .05f;

		float aTime = 0;// t;
		float bTime = t;
		float feedTime = t * .2f;
		float killTime = 0;// t * .2f;

		Vector4 timeVector = new Vector4 ( aTime, bTime, feedTime, killTime );

		m_RDMaterial.SetVector ( "_Timevar", timeVector );

	}


	public Color[] Rates {
		get {

			List<Color> rates = new List<Color> ();

			int numPresets = 0;

			foreach ( RDPresetInfo info in presetsInfo ) {

				Color presetRates = new Color ( info.aRate, info.bRate, info.feedRate, info.killRate );
				rates.Add ( presetRates );
				numPresets++;

			}

			return rates.ToArray ();
		
		}
	}
}
