using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorDebug : MonoBehaviour {

	[System.Serializable]
	public class Prefabs {

		public Console console;
		public FirebaseStatus firebaseStatus;

	}

	public Prefabs prefabs;

	public FirebaseStatus FirebaseStatus { get {
			return prefabs.firebaseStatus;
		}
	}

	public Console Console { get {
			return prefabs.console;
		}
	}


}
