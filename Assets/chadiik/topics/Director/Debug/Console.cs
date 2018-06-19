using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {

	public Queue<string> logsQueue = new Queue<string>();
	public Text textObject;
	public ScrollRect scrollView;

	public void LogString( string value ) {
		logsQueue.Enqueue ( value );
	}

	public void LogLine(string text ) {
		LogString ( "\n" + text );
	}

	protected void FixedUpdate () {
		if(logsQueue.Count > 0 ) {

			string text = logsQueue.Dequeue ();
			textObject.text += text;

			Canvas.ForceUpdateCanvases ();
			scrollView.verticalNormalizedPosition = 0f;

		}
	}


}
