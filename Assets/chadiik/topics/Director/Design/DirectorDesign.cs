using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorDesign : MonoBehaviour {

	[System.Serializable]
	public class Elements {

		public TopicElement topicElement;

	}

	public Elements elements;

	public TopicElement TopicElement { get {
			return elements.topicElement;
		}
	}

	public void FillInfo ( SceneD scene ) {

		TopicElement.SetTitle ( scene.title );
		TopicElement.SetSubtitle ( scene.subtitle );

	}
}
