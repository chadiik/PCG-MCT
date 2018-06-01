using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGrid : Pattern {

	public int cols = 1, rows = 1;
	public float width = 1f, height = 1f;

	public override void Init () {

		float dWidth = width / (float)cols,
			dHeight = height / (float)rows;

		for ( int iRow = 0; iRow < rows; iRow++ ) {

			for ( int iCol = 0; iCol < cols; iCol++ ) {

				float x = (float)iCol * dWidth,
					y = (float)iRow * dHeight;

				Vector3 v3 = new Vector3 ( x, y, 0 );
				vectors.Add ( v3 );

			}

		}

	}

}
