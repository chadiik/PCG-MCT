using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class ObjectArrayExtensions {

	public static int NextIndex(this object[] arr, int i ) {
		return i + 1 >= arr.Length ? 0 : i + 1;
	}

	public static int PreviousIndex ( this object[] arr, int i ) {
		return i - 1 < 0 ? arr.Length - 1 : i - 1;
	}

	public static int NextIndex ( this object [] arr, ref int i ) {
		i = i + 1 >= arr.Length ? 0 : i + 1;
		return i;
	}

	public static int PreviousIndex ( this object [] arr, ref int i ) {
		i = i - 1 < 0 ? arr.Length - 1 : i - 1;
		return i;
	}

}

public class Director : MonoBehaviour {

	public DirectorDebug debugger;
	public DirectorDesign designer;

	[System.Serializable]
	public class Scenes {
		public SceneD jumpToScene;
		public SceneD launcher;
		public SceneD_Intro intro;
		public SceneD_ParticleBoids particleBoids;

		internal int currentSceneIndex = 0;
		private SceneD[] m_SceneOrder;
		internal SceneD[] sceneOrder {
			get {
				if ( m_SceneOrder == null ) m_SceneOrder = new SceneD[] { launcher, intro, particleBoids };
				return m_SceneOrder;
			}
		}
	}

	public Scenes scenes;

	internal FirebaseManager firebase;

	private static bool m_Started = false;

	#region mono

	protected void Awake () {
		
	}

	protected void Start () {

		if ( !m_Started ) {
			DontDestroyOnLoad ( this );

			foreach ( SceneD scene in scenes.sceneOrder ) {
				scene.enabled = false;
			}

			firebase = FirebaseManager.Instance;
			firebase.ExecuteOnInitialisation ( OnFirebaseReady );
		}

	}

	protected void Update () {

		if( m_Started && scenes.jumpToScene != null ) {
			LoadScene ( scenes.jumpToScene );
			scenes.currentSceneIndex = scenes.sceneOrder.ToList ().IndexOf ( scenes.jumpToScene );
			scenes.jumpToScene = null;
		}

	}

	#endregion

	private void OnFirebaseReady () {

		if ( !m_Started ) {
			m_Started = true;

			LoadScene ( scenes.intro );
		}

	}

	public void LoadScene( SceneD scene ) {

		debugger.Console.LogLine ( string.Format ( "Loading scene: {0}", scene.sceneName ) );

		UnityAction<Scene, LoadSceneMode> onSceneLoaded = null;
		onSceneLoaded = ( Scene arg0, LoadSceneMode arg1) => {
			scene.enabled = true;
			SceneManager.sceneLoaded -= onSceneLoaded;
		};
		SceneManager.sceneLoaded += onSceneLoaded;

		UnityAction<Scene> onSceneUnloaded = null;
		onSceneUnloaded = ( Scene arg0) => {
			scene.enabled = false;
			SceneManager.sceneUnloaded -= onSceneUnloaded;
		};
		SceneManager.sceneUnloaded += onSceneUnloaded;

		SceneManager.LoadScene ( scene.sceneName, LoadSceneMode.Single );
		FillInfo ( scene );

	}

	private void FillInfo ( SceneD scene ) {
		designer.FillInfo ( scene );
	}

	public void LoadNextScene () {
		scenes.sceneOrder.NextIndex( ref scenes.currentSceneIndex );
		debugger.Console.LogLine ( string.Format ( "LoadNextScene ({0})", scenes.currentSceneIndex ) );
		LoadScene ( scenes.sceneOrder [ scenes.currentSceneIndex ] );
	}

	public void LoadPreviousScene () {
		scenes.sceneOrder.PreviousIndex( ref scenes.currentSceneIndex );
		debugger.Console.LogLine ( string.Format ( "LoadPreviousScene ({0})", scenes.currentSceneIndex ) );
		LoadScene ( scenes.sceneOrder [ scenes.currentSceneIndex ] );
	}

	public void LoadDefaults () {



	}

	public void LoadDebug () {

		Instantiate ( debugger.FirebaseStatus );

	}

	#region statics

	private static Director c_Instance;
	public static Director Instance {
		get {
			if ( c_Instance == null )
				c_Instance = GameObject.FindObjectOfType<Director> ();

			return c_Instance;
		}
	}

	#endregion

}
