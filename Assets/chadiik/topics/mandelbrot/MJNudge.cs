using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MJNudge : MonoBehaviour {

	public MandlebrotExplorer explorer;
	public float cycleSpeed = 1f;
	public float phase = 0f;

	protected void Update () {

		phase += Time.deltaTime * cycleSpeed;

		float mj = Mathf.Clamp01 ( Mathf.Cos ( phase ) );
		mj = mj * mj * ( 3 - 2 * mj );

		explorer.mandelbrotMJRatio = mj;

	}


}
