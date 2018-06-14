#if FIREBASE
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System.Threading.Tasks;
#else
using FirebaseInterface;
using FirebaseInterface.Database;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirebaseManager : MonoBehaviour {

	private static FirebaseManager m_Instance;
	public static FirebaseManager Instance {
		get {

			if ( m_Instance == null ) {

				#if FIREBASE
				m_Instance = GameObject.FindObjectOfType<FirebaseManager> ();
#endif

			}

			return m_Instance;
		}
	}

	private DatabaseReference m_RootReference;
	private DatabaseReference m_Database;
	private string m_UID;
	private bool m_Initialised;

	public delegate void FBEvent ();
	public FBEvent OnInitialised;

	public delegate void ProcessValue ( object value );

	protected void Start () {

		#if FIREBASE
		Init ();
#endif

	}

	private void Init () {

		m_Initialised = true;

		string datePrefix = System.DateTime.UtcNow.ToString ( "yyMMdd"/*-HHmmss"*/ );
		m_UID = datePrefix;

		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ( "https://pcgtalk-81074.firebaseio.com/" );
		m_RootReference = FirebaseDatabase.DefaultInstance.RootReference;
		m_Database = m_RootReference.Child ( m_UID );

		if ( OnInitialised != null ) {

			OnInitialised ();

		}

	}

	public void ExecuteOnInitialisation ( FBEvent callback ) {

		if ( m_Initialised ) {

			if (callback != null)
				callback ();

		}
		else {

			OnInitialised += callback;

		}

	}

	private DatabaseReference ResolveDBPath ( string dbPath ) {

		string[] children = dbPath.Split ( '/' );
		DatabaseReference child = m_Database;

		for (int i = 0, numChildren = children.Length; i < numChildren; i++) {

			child = child.Child ( children[ i ] );

		}

		return child;

	}

	public DatabaseReference Path ( string dbPath ) {

		return m_Database.Child ( dbPath );

	}

	public Task SetValueAsync( string dbPath, object value ){

		Task task = m_Database.Child ( dbPath ).SetValueAsync ( value );

		return task;

	}

	public void GetValueAsync ( string dbPath, ProcessValue callback ) {

		m_Database.Child ( dbPath ).GetValueAsync ().ContinueWith (
			task => {

				if ( task.IsFaulted ) {

					// Handle the error...

				}
				else if ( task.IsCompleted ) {

					DataSnapshot snapshot = task.Result;
					callback ( snapshot.Value );

				}

			}
		);

	}
}