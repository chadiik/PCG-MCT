using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CSVData : MonoBehaviour {

	public TextAsset resource;

	private Dictionary<string, List<string>> m_Database;

	public Dictionary<string, List<string>> Data {

		get {

			if ( m_Database == null ) {

				CreateData ();

			}

			return m_Database;

		}

	}

	protected void Start () {

		if ( m_Database == null ) {

			CreateData ();

		}

	}

	private void CreateData () {

		//Debug.LogFormat ( "resource: {0}", resource.text );
		string[] lineDelim = new[] { "\r\n", "\r", "\n" };
		string[] lines = resource.text.Split ( lineDelim, System.StringSplitOptions.None );

		int numLines = lines.Length;

		if ( numLines == 0 ) {

			Debug.LogWarningFormat ( "resource seems empty, lines.length = {0}", numLines );
			return;

		}

		m_Database = new Dictionary<string, List<string>> ();

		string[] keys = lines[ 0 ].Split ( ',' );
		//Debug.LogFormat ( "keys: {0}", keys.ArrayToString () );

		int numKeys = keys.Length;

		for ( int iKey = 0; iKey < numKeys; iKey++ ) {

			Debug.LogFormat ( "key[{0}] = {1}", iKey, keys[ iKey ] );
			m_Database.Add ( keys[ iKey ], new List<string>() );

		}

		char[] separator = new char[] { ',' };

		for ( int iLine = 1; iLine < numLines; iLine++ ) {

			string[] values = lines[ iLine ].Split ( separator, System.StringSplitOptions.None );
			//Debug.LogFormat ( "values: {0}", values.ArrayToString () );

			if ( values.Length == numKeys ) {

				for ( int iKey = 0; iKey < numKeys; iKey++ ) {

					m_Database[ keys[ iKey ] ].Add( values[ iKey ] );

					if (iLine == 1) Debug.Log(values[iKey]);

				}

			}

		}

	}

	public void Test( int entries, string[] keys ) {

		StringBuilder stringBuilder = new StringBuilder();

		for(int i = 0; i < entries; i++) {

			for(int iPrint = 0; iPrint < keys.Length; iPrint++) {

				string key = keys[iPrint].ToString();
				Debug.LogFormat("Test key: {0}", key);
				stringBuilder.AppendLine( string.Format("{0}: {1}", key, Data[key][i].ToString()) );

			}

			stringBuilder.AppendLine(".");

		}

		System.IO.File.WriteAllText("CSVDataTest.txt", stringBuilder.ToString());

	}

	public void Test() {

		StringBuilder stringBuilder = new StringBuilder();

		string[] keys = Data.Keys.ToArray();
		int numKeys = keys.Length;
		int numEntries = Data[keys[0]].Count;
		for (int i = 0; i < numEntries; i++) {

			for (int iPrint = 0; iPrint < numKeys; iPrint++) {

				string key = keys[iPrint].ToString();
				stringBuilder.AppendLine(string.Format("{0}: {1}", key, Data[key][i].ToString()));

			}

			stringBuilder.AppendLine(".");

		}

		System.IO.File.WriteAllText("CSVDataTest.txt", stringBuilder.ToString());

	}

}
