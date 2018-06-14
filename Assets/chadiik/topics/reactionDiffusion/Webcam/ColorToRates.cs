using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorToRates : MonoBehaviour {

	public Material colorMaterial;
	public Material ratesMaterial;
	public RenderTexture result;

	[Header ( "Rates" )]
	public MonoBehaviour ratesAction;
	public Color[] rates;

	protected void Start () {

		rates = ( ratesAction as IRates ).Rates;
		Debug.Log ( rates.ArrayToString () );

		int numRates = rates.Length;

		Texture2D ratesTexture = new Texture2D ( numRates, 1, TextureFormat.RGBAFloat, false, true );
		ratesTexture.filterMode = FilterMode.Trilinear;
		ratesTexture.anisoLevel = 4;

		ratesTexture.SetPixels ( rates );
		ratesTexture.Apply ();

		//ratesMaterial.mainTexture = colorMaterial.mainTexture;
		ratesMaterial.SetTexture ( "_Rates", ratesTexture );

		result = new RenderTexture ( colorMaterial.mainTexture.width, colorMaterial.mainTexture.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear );
		GetComponent<Renderer>().sharedMaterial.mainTexture = result;

	}

	protected void FixedUpdate () {

		Graphics.Blit ( colorMaterial.mainTexture, result, ratesMaterial );

	}

	/////

	public interface IRates {

		Color[] Rates { get; }

	}

}
