using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOnRenderImage : MonoBehaviour {

	public Material imageEffectMaterial;

	protected void OnRenderImage ( RenderTexture source, RenderTexture destination ) {

		Graphics.Blit ( source, destination, imageEffectMaterial );

	}

}
