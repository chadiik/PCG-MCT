using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ApplicationProcess : MonoBehaviour {

	public static string vlcPath = "C:/Program Files (x86)/VideoLAN/VLC/vlc.exe";
	public static System.Diagnostics.Process lastOpenProcess;

	public static string ProjectDir (string fileName ) {
		return Directory.GetParent ( Application.dataPath ).FullName + "/" + fileName;
	}

	public static System.Diagnostics.Process PlayVideo ( string filePath) {

		Debug.LogFormat ( "PlayVideo {0}", filePath );

		string arguments = "";
		arguments += " -f";
		arguments += " \"file:///" + filePath + "\"";
		arguments += " vlc://quit";

		Debug.LogFormat ( "arguments = {0}", arguments );

		lastOpenProcess = System.Diagnostics.Process.Start ( vlcPath, arguments );
		return lastOpenProcess;
	}

	public static bool CloseLastOpenProcess () {

		if(lastOpenProcess != null ) {
			lastOpenProcess.CloseMainWindow ();
			lastOpenProcess = null;
			return true;
		}

		return false;

	}

	public string filePath;

	public bool closeCurrentProcess = false;

	protected void Start () {

		string path = ApplicationProcess.ProjectDir(filePath);
		ApplicationProcess.PlayVideo ( path );

	}

	protected void Update () {

		if ( closeCurrentProcess ) {
			closeCurrentProcess = false;
			ApplicationProcess.CloseLastOpenProcess ();
		}

	}

}
