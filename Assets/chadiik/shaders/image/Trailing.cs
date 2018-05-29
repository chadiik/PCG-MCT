using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailing : MonoBehaviour {

	public Shader shader;
	[Range(0, 1)]
	public float intensity;

	public Material m_Material;
	private RenderTexture m_OldTexPing, m_OldTexPong;
	private int m_PingPong = 0;

	// Creates a private material used to the effect
	void Awake () {

		m_Material = new Material ( shader );

	}

	// Postprocess the image
	void OnRenderImage ( RenderTexture source, RenderTexture destination ) {

		if ( intensity == 0 ) {

			Graphics.Blit ( source, destination );
			return;

		}

		bool ping = ((m_PingPong++) % 2) ==0;

		if ( m_OldTexPing == null ) {

			m_Material.SetTexture ( "_OldTex", source );

			m_OldTexPing = new RenderTexture ( source );
			m_OldTexPong = new RenderTexture ( source );

			m_OldTexPing.Create ();
			m_OldTexPong.Create ();

		}
		else {

			m_Material.SetTexture ( "_OldTex", ping ? m_OldTexPing : m_OldTexPong );

		}

		m_Material.SetFloat ( "_Blend", intensity );

		Graphics.Blit ( source, destination, m_Material );

		//m_OldTex = destination;
		if ( m_OldTexPing.IsCreated() && m_OldTexPong.IsCreated() ) {

			//Graphics.CopyTexture ( source, ping ? m_OldTexPong : m_OldTexPing );
			Graphics.Blit ( source, ping ? m_OldTexPong : m_OldTexPing, m_Material );

		}

	}

}