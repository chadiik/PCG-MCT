using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleCA : MonoBehaviour {

	[Header("Simulation")]
	public CustomRenderTexture texture;
	public Material seedingMaterial;
	public int updateRate = 1;
	public float delay = 1;
	public float frameDelay = .5f;

	public float texSize = 1f;

	[Header("Presets")]
	public Texture2D presets8;
	public int next;
	public Texture2D tex32;
	private Color[] presets8Colors;
	internal int m_CurrentIndex = -1;


	[Header("Presentation")]
	public Material displayMaterial;

	protected Material m_SimMaterial;
	private Vector2 m_StartingTexSize;
	private int m_TexWidth, m_TexHeight;

	protected virtual void Start () {

		m_SimMaterial = texture.material;
		m_StartingTexSize = new Vector2 ( m_TexWidth = texture.width, m_TexHeight = texture.height );

		texture.Initialize ();

		StartCoroutine ( UpdateSimulation () );

	}

	protected virtual void FixedUpdate () {

		if ( texSize > .5f ) {

			int newTexWidth = Mathf.FloorToInt(m_StartingTexSize.x * texSize);
			int newTexHeight = Mathf.FloorToInt(m_StartingTexSize.y * texSize);

			if ( newTexWidth != m_TexWidth && newTexHeight != m_TexHeight ) {
				m_TexWidth = newTexWidth;
				m_TexHeight = newTexHeight;

				CustomRenderTexture crt = new CustomRenderTexture(newTexWidth, newTexHeight, texture.format);
				crt.material = m_SimMaterial;
				crt.updateMode = texture.updateMode;
				crt.doubleBuffered = texture.doubleBuffered;
				crt.initializationMode = texture.initializationMode;
				crt.initializationSource = CustomRenderTextureInitializationSource.TextureAndColor;
				crt.initializationTexture = texture;

				crt.Initialize ();

				crt.initializationSource = texture.initializationSource;
				if ( crt.initializationSource == CustomRenderTextureInitializationSource.Material )
					crt.initializationMaterial = texture.initializationMaterial;

				texture = crt;
				displayMaterial.mainTexture = texture;

			}
		}

		if(next != 0 ) {
			m_CurrentIndex = m_CurrentIndex + next;
			next = 0;

			if ( m_CurrentIndex >= 16 ) m_CurrentIndex = 0;
			if ( m_CurrentIndex < 0 ) m_CurrentIndex = 15;

			if( tex32 == null ) {
				tex32 = new Texture2D ( 32, 32 );
				tex32.filterMode = FilterMode.Point;

				presets8Colors = presets8.GetPixels ();
			}

			Color[] tex32Colors = new Color[32 * 32];
			for ( int i = 0; i < presets8Colors.Length; i++ ) {
				tex32Colors [ i ] = Color.black;
			}

			int corner = 10;
			for ( int y = 0; y < 8; y++ ) {
				for ( int x = 0; x < 8; x++ ) {
					int ni = m_CurrentIndex;
					int sourceY = 31 - (ni / 4) * 8 + y;
					int sourceX = (ni % 4) * 8 + x;
					if ( ( x == 0 && y == 0 ) || ( x == 7 && y == 7 ) ) Debug.LogFormat ( "{0}, {1} x {2}", m_CurrentIndex, sourceX, sourceY );
					int colorIndex = (y + corner) * 32 + x + corner;
					tex32Colors [ colorIndex ] = presets8.GetPixel ( sourceX, sourceY );
				}
			}

			tex32.SetPixels ( tex32Colors );

			byte[] png = tex32.EncodeToPNG (  );
			string pngPath = "Assets/tex32.jpg";
			System.IO.File.WriteAllBytes ( pngPath, png );

			AssetDatabase.Refresh ();

			if ( texture.initializationSource != CustomRenderTextureInitializationSource.TextureAndColor )
				texture.initializationSource = CustomRenderTextureInitializationSource.TextureAndColor;

			texture.initializationTexture = ( Texture2D ) AssetDatabase.LoadMainAssetAtPath ( pngPath ); ;
			texture.Initialize ();
			frameDelay = .5f;

		}

	}

	private IEnumerator UpdateSimulation () {

		while ( true ) {

			if ( updateRate > 0 ) {

				texture.Update ( updateRate );

				if ( (frameDelay + delay) > 1f / 120f )
					yield return new WaitForSeconds ( frameDelay + delay );
				else
					yield return new WaitForFixedUpdate ();

				frameDelay = 0f;

			}
		}
	}

	protected virtual void Update () {



	}

}
