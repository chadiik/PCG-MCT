using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTMaterial : MonoBehaviour {

	public Material material;
	public RenderTexture result;
	public Texture source;

	private Material m_Material;

	protected void Start () {

		if ( result == null ) {

			result = new RenderTexture ( 2048, 2048, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default );

		}

		m_Material = GetComponent<Renderer> ().sharedMaterial;
		m_Material.mainTexture = result;

		if ( source == null ) {

			source = new Texture2D ( 1, 1 );

		}


	}

	protected void FixedUpdate () {

		Graphics.Blit ( source, result, material );

	}

}
