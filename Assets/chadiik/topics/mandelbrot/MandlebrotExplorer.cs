using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandlebrotExplorer : MonoBehaviour {

	public Material mandelbrotMaterial;
	public Material juliaMaterial;

	[Header("Area")]

	[Range ( -1, 1 )]
	public float x;

	[Range ( -1, 1 )]
	public float y;

	[Range(0, 4)]
	public float scale;

	[Range ( -Mathf.PI, Mathf.PI )]
	public float rotation;

	[Header("Fractal shape")]

	public float yMul = 2;

	[Range(0, 1)]
	public float mandelbrotMJRatio = 1;

	[Range ( 0, 1 )]
	public float juliaMJRatio = 0;

	[Header ( "Speeds" )]
	public float timeSpeed = .05f;
	public float scaleFactor = 1.01f;
	public float rotateOffset = .03f;

	public float smooth = .1f;

	private Vector4 m_Area = new Vector4 ();
	private float m_Scale = 0f, m_Angle = 0f;

	private void UpdateValues () {

		float t = Time.time;
		yMul = t * timeSpeed;

	}

	public void UpdateMaterial (Material material) {

		m_Angle = Mathf.Lerp ( m_Angle, rotation, smooth );
		m_Scale = Mathf.Lerp ( m_Scale, scale, smooth );

		float aspect = ( float )Screen.width / ( float )Screen.height;

		float scaleX = m_Scale, scaleY = m_Scale;
		
		if ( aspect > 1f ) {

			scaleY /= aspect;
		
		}
		else {

			scaleX *= aspect;

		}

		m_Area.Set ( Mathf.Lerp ( m_Area.x, x, smooth ), Mathf.Lerp ( m_Area.y, y, smooth ), scaleX, scaleY );
		material.SetVector ( "_Area", m_Area );
		material.SetFloat ( "_Angle", m_Angle );
		material.SetFloat ( "_YMul", yMul );

	}

	public void Focus ( float x, float y ) {

		this.x = x;
		this.y = y;

	}

	private void ZoomIn () {

		scale /= scaleFactor;

	}

	private void ZoomOut () {

		scale *= scaleFactor;

	}

	private void Rotate ( ref Vector2 p, float angle ) {

		float s = Mathf.Sin ( angle );
		float c = Mathf.Cos ( angle );
		float x = p.x * c - p.y * s;
		float y = p.x * s + p.y * c;
		p.x = x;
		p.y = y;

	}

	private void HandleMouseMove () {

		Vector2 mouseViewport = new Vector2 (
			Input.mousePosition.x / ( float )Screen.width - .5f,
			Input.mousePosition.y / ( float )Screen.height - .5f 
		);

		Rotate ( ref mouseViewport, rotation );
		Vector2 position = new Vector2(x, y) + mouseViewport * scale * .03f;

		Focus ( position.x, position.y );

	}

	private void HandleInputs () {

		if ( Input.GetMouseButton ( 0 ) ) {

			HandleMouseMove ();

		}

		if ( Input.GetKey(KeyCode.W)){

			ZoomIn ();
			
		}

		if ( Input.GetKey ( KeyCode.S ) ) {

			ZoomOut ();

		}

		if ( Input.GetKey ( KeyCode.A ) ) {

			rotation -= rotateOffset;

		}

		if ( Input.GetKey ( KeyCode.D ) ) {

			rotation += rotateOffset;

		}

		

		float scroll = Input.GetAxis ( "Mouse ScrollWheel" );
		if ( Mathf.Abs(scroll) > .01f ) {

			rotation += Mathf.Sign ( scroll ) * rotateOffset;

		}



	}

	protected void FixedUpdate () {

		HandleInputs ();
		UpdateValues ();
		UpdateMaterial ( mandelbrotMaterial );
		UpdateMaterial ( juliaMaterial );

		mandelbrotMaterial.SetFloat ( "_MJRatio", mandelbrotMJRatio );
		juliaMaterial.SetFloat ( "_MJRatio", juliaMJRatio );

	}
}
