using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseStatus : MonoBehaviour {

	public Text textObject;
	public ScrollRect scrollView;
	internal int prevTextLength = 0;

	protected void FixedUpdate () {

		int charCount = textObject.cachedTextGeneratorForLayout.characterCount;
		if ( charCount != prevTextLength ) {
			prevTextLength = charCount;
			Canvas.ForceUpdateCanvases ();
			scrollView.verticalNormalizedPosition = 0f;
		}
	}

}
