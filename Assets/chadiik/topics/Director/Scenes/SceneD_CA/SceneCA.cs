using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCA : MonoBehaviour {

	public static SceneCA instance;

	public enum Step { Idle, Repeat, Next, BufferEnum, LargerRandom, RandomizeLarger, Repeat2, Exit };
	public Step step;

	public SimpleCA startingQuad, largerRandom;
	public CameraControllerPreset largerRandomView;

	protected void Awake () {
		if ( instance == null )
			instance = this;
	}

	protected void Start () {

		StartCoroutine ( ScenarioCoroutine () );

	}

	private IEnumerator ScenarioCoroutine () {

		int numQuadClones = 0;
		SimpleCA currentQuad = startingQuad;
		List< SimpleCA > startingQuadClones = new List<SimpleCA>();
		startingQuadClones.Add ( startingQuad );
		while ( isActiveAndEnabled ) {
			switch ( step ) {

				case Step.Idle: {
						break;
					}

				case Step.Repeat: {
						break;
					}

				case Step.Next: {

						GameObject clone = Instantiate<GameObject>(currentQuad.gameObject);
						SimpleCA ca = clone.GetComponent<SimpleCA>();
						startingQuadClones.Add ( ca );
						ca.texture = Instantiate<CustomRenderTexture> ( startingQuad.texture );
						Material newMat = ca.GetComponent<Renderer> ().sharedMaterial = Instantiate<Material> ( startingQuad.GetComponent<Renderer> ().sharedMaterial );
						newMat.mainTexture = ca.texture;
						ca.m_CurrentIndex = numQuadClones - 1;
						ca.next = 1;
						numQuadClones++;

						Vector3 newPos = ca.transform.position;
						newPos.x += currentQuad.transform.localScale.x * numQuadClones * 1.4f;
						ca.transform.position = newPos;

						Vector3 camPos = OrbitController.Instance.TargetPosition;
						camPos.x = newPos.x;
						OrbitController.Instance.TargetPosition = camPos;
						OrbitController.Instance.orbitTarget = ca.transform;

						step = Step.Repeat;
						break;
					}

				case Step.LargerRandom: {

						foreach ( SimpleCA clone in startingQuadClones ) {
							Destroy ( clone.gameObject );
						}
						startingQuadClones.Clear ();

						largerRandom.gameObject.SetActive ( true );
						OrbitController.Instance.orbitTarget = largerRandom.transform;
						OrbitController.Instance.Apply ( largerRandomView );


						step = Step.Idle;
						break;
					}

				case Step.RandomizeLarger: {

						Material initMat = largerRandom.texture.initializationMaterial;
						if(initMat != null ) {
							float freq = initMat.GetFloat("_Frequency");
							freq += .1f;
							freq = Mathf.Min ( .8f, freq );
							initMat.SetFloat ( "_Frequency", freq );
						}

						largerRandom.texture.Initialize ();
						largerRandom.frameDelay = .5f;

						step = Step.Idle;
						break;
					}

				case Step.Exit: {
						Director.Instance.LoadNextScene ();
						break;
					}

			}

			yield return new WaitForFixedUpdate ();
		}

	}
}

