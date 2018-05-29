using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawScreenLine : MonoBehaviour {

	private static Line c_Line;

	private static Line Line {

		get {

			if ( c_Line == null ) {

				c_Line = GameObject.FindObjectOfType<Line> ();

			}

			return c_Line;

		}

	}

	private static Camera Camera {

		get {

			return Camera.main;

		}

	}

	private static Vector2 ToViewportSpace ( Vector3 position ) {


		Vector3 screen = Camera.WorldToScreenPoint ( position );

		return new Vector2 ( screen.x / Camera.pixelWidth, screen.y / Camera.pixelHeight );

	}

	public static void StrokeLine ( Vector3 a, Vector3 b ) {

		Line line = Line;

		line.pointA = ToViewportSpace ( a );
		line.pointB = ToViewportSpace ( b );

	}


	private static Vector3 c_LineFrom;
	public static void StrokeLineFrom ( Vector3 a ) {

		c_LineFrom = ToViewportSpace ( a );

	}

	public static void StrokeLineTo ( Vector3 a ) {

		Line line = Line;

		line.pointA = c_LineFrom;
		c_LineFrom = line.pointB = ToViewportSpace ( a );

	}
}
