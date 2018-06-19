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
using UnityEngine.UI;

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

	private static bool isAdmin = false;
	private static bool adminCheck = false;
	public static bool IsAdmin {
		get {

			if ( !adminCheck )
				Instance.ScheduleLog ( "Admin check not completed yet!" );

			return isAdmin;
		}
		set {
			adminCheck = true;
			isAdmin = true;
		}
	}

	public string id;
	public string MACFilter;

	private DatabaseReference m_RootReference;
	private DatabaseReference m_Database;
	private bool m_Initialised;

	public delegate void FBEvent ();
	public Queue<FBEvent> OnInitialised = new Queue<FBEvent>();
	private bool m_NotifyInitialised = false;

	private bool m_Init = false;

	public delegate void ProcessValue ( object value );

	protected void Awake () {

		if ( !enabled )
			return;

		Init ();

	}

	private void Init () {

		if ( !string.IsNullOrEmpty ( MACFilter ) && MACFilter [ 0 ] != '!' ) {

			adminCheck = true;
			isAdmin =
				MACFilter == NetworkConnection.GetMACAddress ();

		}

		VisualLog ( "IsAdmin: " + IsAdmin );

		DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

		FirebaseApp.CheckAndFixDependenciesAsync ().ContinueWith ( task => {

			dependencyStatus = task.Result;
			ScheduleLog ( dependencyStatus.ToString () );

			if ( dependencyStatus == DependencyStatus.Available ) {

				if ( IsAdmin ) {
					CreateNew ( id );
				}
				else {
					id = null;
					InitDatabase ();
				}

			}
			else {

				ScheduleLog ( "Could not resolve!" );

			}
		} );

	}

	protected void Update () {

		if( m_ScheduledLog.Count > 0 ) {

			VisualLog ( m_ScheduledLog.Dequeue() );

		}

		if ( m_NotifyInitialised ) {

			VisualLog ( "FirebaseManager ready!" );

			m_NotifyInitialised = false;
			if ( OnInitialised.Count > 0 ) {

				OnInitialised.Dequeue ().Invoke ();

			}

		}

		if ( Input.GetKeyDown ( KeyCode.Escape ) && Application.platform == RuntimePlatform.Android ) {

			Application.Quit ();

		}

		/*
		if( ( ( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began ) || Input.GetMouseButtonDown ( 0 ) )
			&& !m_Init 
		) {

			m_Init = true;
			Init ();

		}
		*/

	}

	public FirebaseManager CreateNew ( string id ) {

#if UNITY_EDITOR
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ( "https://pcgtalk-81074.firebaseio.com/" );
		if ( FirebaseApp.DefaultInstance.Options.DatabaseUrl != null )
			FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ( FirebaseApp.DefaultInstance.Options.DatabaseUrl );
#endif

		m_RootReference = FirebaseDatabase.DefaultInstance.RootReference;
		DatabaseReference current = string.IsNullOrEmpty(id) ? m_RootReference.Push() : m_RootReference.Child ( id );

		m_RootReference.Child ( "current" ).SetValueAsync ( current.Key ).ContinueWith(
			
			task => {

				if ( task.IsFaulted ) {

					ScheduleLog ( "Error in CreateNew" );

				}
				else if ( task.IsCompleted ) {

					InitDatabase ( /*current.Key*/ );

				}
				

			}

		);

		return this;

	}

	public void InitDatabase ( string id = null ) {

		ScheduleLog ( string.Format ( "InitDatabase with {0}", id ) );

		if ( m_RootReference == null ) {

			ScheduleLog ( string.Format ( "InitDatabase creates m_RootReference" ) );

#if UNITY_EDITOR
			FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ( "https://pcgtalk-81074.firebaseio.com/" );
			if ( FirebaseApp.DefaultInstance.Options.DatabaseUrl != null )
				FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ( FirebaseApp.DefaultInstance.Options.DatabaseUrl );
#endif

			m_RootReference = FirebaseDatabase.DefaultInstance.RootReference;

		}

		ScheduleLog ( string.Format( "{0}", m_RootReference.ToString() ) );

		if ( string.IsNullOrEmpty ( id ) ) {

			m_RootReference.Child ( "current" ).GetValueAsync ().ContinueWith (

				task => {

					if ( task.IsFaulted ) {

						ScheduleLog ( "Error in InitDatabase" );

					}
					else if ( task.IsCompleted ) {

						ScheduleLog ( task.Result.Value == null ? "Null Result" : task.Result.Value.ToString () );

						id = ( string ) task.Result.Value;

						if ( ! string.IsNullOrEmpty ( id ) )
							SetDatabase ( id );

					}

				}

			);

		}
		else {

			SetDatabase ( id );

		}

	}

	private void SetDatabase ( string id = null ) {

		ScheduleLog ( string.Format ( "SetDatabase with {0}", id ) );

		this.id = id;
		m_Database = m_RootReference.Child ( this.id );

		m_Initialised = true;
		m_NotifyInitialised = true;

	}

	public void ExecuteOnInitialisation ( FBEvent callback ) {

		OnInitialised.Enqueue ( callback );

		if ( m_Initialised ) {

			m_NotifyInitialised = true;

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

	private struct ListenHandler {
		public string dbPath;
		public EventHandler<ValueChangedEventArgs> handler;
	}
	private Dictionary<ProcessValue, ListenHandler> m_Listeners = new Dictionary<ProcessValue, ListenHandler>();
	public void Listen ( string dbPath, ProcessValue callback ) {

		ListenHandler handler = new ListenHandler(){
			dbPath = dbPath,
			handler = ( object sender, ValueChangedEventArgs e) => {
				callback ( e.Snapshot.Value );
			}
		};

		m_Listeners.Add ( callback, handler );

		m_Database.Child ( dbPath ).ValueChanged += handler.handler;
	}

	public void StopListen ( ProcessValue callback ) {

		ListenHandler handler;
		if(m_Listeners.TryGetValue(callback, out handler ) ) {
			m_Database.Child ( handler.dbPath ).ValueChanged -= handler.handler;
		}

	}

	private Queue<string> m_ScheduledLog = new Queue<string>();
	private int m_LogIndex = 0;
	private void ScheduleLog(string value ) {
		m_ScheduledLog.Enqueue ( string.Format("{0}: {1}", m_LogIndex++, value) );
	}

	public Text visualLog;
	public void VisualLog(string value ) {

		//value = string.Format ( "{0}: {1}", m_LogIndex++, value );

		if ( visualLog != null ) {

			visualLog.text += "\n" + value;

		}
		else {

			Debug.LogFormat ( "VisualLog: {0}", value );

		}

	}

}
