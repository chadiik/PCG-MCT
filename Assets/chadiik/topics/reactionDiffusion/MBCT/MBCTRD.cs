using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBCTRD : ReactionDiffusionMapped {

	[Header("Masked")]
	public Texture2D killMask;

	private Color[] m_KillMaskColors;

	protected override void Start () {

		m_KillMaskColors = killMask.GetPixels ();

		base.Start ();

	}

	protected override Color CreateConvolutionColor ( float convCell, float convAdj, float convDiag, float deltaTime, float u, float v ) {

		int index = ( int )( v * ( float )killMask.height ) * killMask.width + ( int )( u * ( float )killMask.width );
		float value = m_KillMaskColors[ index ].r;

		/*convCell = convCell * value;
		convAdj = convAdj * value;
		convDiag = convDiag * value;*/
		//deltaTime = deltaTime * value;

		return base.CreateConvolutionColor ( convCell, convAdj, convDiag, deltaTime, u, v );
	}

	protected override Color CreateRatesColor ( float aRate, float bRate, float feedRate, float killRate, float u, float v ) {

		int index = ( int )( v * ( float )killMask.height ) * killMask.width + ( int )( u * ( float )killMask.width );
		float value = m_KillMaskColors[ index ].r;

		aRate = aRate * value;
		bRate = bRate * value;
		feedRate = feedRate * Mathf.Pow ( value, 2f );
		killRate = killRate * value + ( 1 - value );
		
		return base.CreateRatesColor ( aRate, bRate, feedRate, killRate, u, v );

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

}
