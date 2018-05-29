using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Line : MonoBehaviour {

	public Shader shader;

	public Vector3 pointA, pointB;
	public Color color;
	public float thickness = 100;

	private Material m_Material;
	private RenderTexture m_OldTexPing, m_OldTexPong;
	private int m_PingPong = 0;

	// Creates a private material used to the effect
	void Awake () {

		m_Material = new Material ( shader );

	}

	// Postprocess the image
	void OnRenderImage ( RenderTexture source, RenderTexture destination ) {

		if ( color.a == 0 ) {

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

		m_Material.SetVector ( "_A", pointA );
		m_Material.SetVector ( "_B", pointB );
		m_Material.SetColor ( "_Color", color );
		m_Material.SetFloat ( "_Thickness", thickness );

		Graphics.Blit ( source, destination, m_Material );

		//m_OldTex = destination;
		if ( m_OldTexPing.IsCreated() && m_OldTexPong.IsCreated() ) {
			Debug.Log ( ping );
			//Graphics.CopyTexture ( source, ping ? m_OldTexPong : m_OldTexPing );
			Graphics.Blit ( source, ping ? m_OldTexPong : m_OldTexPing, m_Material );
		}

	}

}