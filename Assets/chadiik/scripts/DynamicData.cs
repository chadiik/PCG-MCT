using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DynamicData {

	[System.Serializable]
	public class Data {

		public string key;
		public float value;

	}

	public string dataString = "a=1,b=2!";

	[SerializeField]
	public List<Data> dataList = new List<Data> ();

	public DynamicData ( string dataString ) {

		this.dataString = dataString;
		ParseInput ();

	}

	private void ParseInput () {

		if ( dataString.Length > 1 && dataString.IndexOf ( '!' ) == dataString.Length - 1 ) {

			dataString = dataString.Remove ( dataString.Length - 1, 1 );

			string[] variablesDef = dataString.Split ( ',' );
			
			foreach ( string variableDef in variablesDef ) {

				string[] components = variableDef.Split ( '=' );

				Set ( components[ 0 ], float.Parse ( components[ 1 ] ) );

			}

			dataString = "";

		}

	}

	public Data Set ( string key, float value ) {

		Data data = Get ( key );

		if ( data == null ) {

			data = new Data () { key = key, value = value };
			dataList.Add ( data );

		}
		else {

			data.value = value;

		}

		return data;

	}

	public Data Get ( string key ) {

		foreach ( Data item in dataList ) {

			if ( item.key == key ) return item;

		}

		return null;

	}

	protected void Update () {

		ParseInput ();

	}

}
