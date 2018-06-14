using UnityEngine;
using System.Collections;
using ArabicSupport;

public class Fix3dTextCS : MonoBehaviour {
	
	public string text;
	public bool autoFix = true;
	public bool tashkeel = true;
	public bool hinduNumbers = true;

	private TextMesh m_TextMesh;
	
	protected void Start () {

		m_TextMesh = gameObject.GetComponent<TextMesh> ();

	}

	public void ReflectChanges () {

		text = ArabicFixer.Fix ( m_TextMesh.text, tashkeel, hinduNumbers );
		m_TextMesh.text = text;

	}
	
	protected void FixedUpdate () {

		if ( m_TextMesh.text != text ) {

			ReflectChanges ();

		}
	
	}
}
