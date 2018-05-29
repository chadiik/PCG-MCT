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

	public float scaleFactor = 1.01f;
	public float rotateOffset = .03f;

	private void UpdateValues () {

		float t = Time.time;
		yMul = 2 + Mathf.Cos ( t * .1f ) * .05f;

	}

	public void UpdateShader (Material material) {

		float aspect = ( float )Screen.width / ( float )Screen.height;

		float scaleX = scale, scaleY = scale;
		
		if ( aspect > 1f ) {

			scaleY /= aspect;
		
		}
		else {

			scaleX *= aspect;

		}

		Vector4 area = new Vector4 ( x, y, scaleX, scaleY );
		material.SetVector ( "_Area", area );
		material.SetFloat ( "_Angle", rotation );
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
		Vector2 position = new Vector2(x, y) + mouseViewport * scale * .05f;

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
		UpdateShader ( mandelbrotMaterial );
		UpdateShader ( juliaMaterial );

		mandelbrotMaterial.SetFloat ( "_MJRatio", mandelbrotMJRatio );
		juliaMaterial.SetFloat ( "_MJRatio", juliaMJRatio );

	}
}
