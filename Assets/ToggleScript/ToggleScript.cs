#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ToggleScript : MonoBehaviour {

	public ToggleTemplate target;
	public bool enable;

	protected void Update () {

		if ( target.enabled != enable ) {

			string[] symbols = target.define.Split ( ',' );

			foreach ( BuildTargetGroup platform in target.platforms ) {

				string symbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup ( platform );
				List<string> existingSymbols = symbolsString.Split ( ';' ).ToList<string> ();

				foreach ( string symbol in symbols ) {

					int index = existingSymbols.IndexOf ( symbol );
					bool containSymbol = index != -1;

					if ( enable && !containSymbol ) {

						Debug.Log ( "Adding #define: '" + symbol + "' to " + platform.ToString () );

						existingSymbols.Add ( symbol );

					}

					if ( !enable && containSymbol ) {

						Debug.Log ( "Removing #define: '" + symbol + "' from " + platform.ToString () );

						existingSymbols.RemoveAt ( index );

					}

				}

				symbolsString = String.Join ( ";", existingSymbols.ToArray () );
				PlayerSettings.SetScriptingDefineSymbolsForGroup ( platform, symbolsString );

			}

			string[] paths = target.path.Split ( ',' );

			foreach ( string path in paths ) {

				string oldPath = path + ( target.enabled ? string.Empty : "~" );

				bool pathEnabled = oldPath.IndexOf ( '~' ) == -1;
				string newName = "";
				if ( enable && !pathEnabled ) newName = path;
				if ( !enable && pathEnabled ) newName = path + "~";

				string newPath = newName;
				Debug.LogFormat ( "Renaming {0} to {1}", oldPath, newPath );

				string assetsFolder = Application.dataPath + "/";
				try {

					string source = assetsFolder + oldPath;
					Directory.Move ( source, assetsFolder + newPath );

					Directory.Delete ( source );

				}
				catch ( Exception e ) {

					e.ToString ();

				}

			}

			target.enabled = enable;

			AssetDatabase.Refresh ();

		}

	}

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptsReloaded() {

		return;

	}

}

#endif