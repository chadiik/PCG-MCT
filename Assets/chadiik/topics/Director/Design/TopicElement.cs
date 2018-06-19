using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicElement : MonoBehaviour {

	public Text titleText, subtitleText;

	protected void Start () {
		
	}

	public void SetTitle(string text ) {

		titleText.text = text;

	}

	public void SetSubtitle(string text ) {

		subtitleText.text = text;

	}

}
