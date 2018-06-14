using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVHelperData : MonoBehaviour {

	[System.Serializable]
	public enum Descriptor { Lyrics };

	[SerializeField]
	public Descriptor descriptor;
	public string path = "Assets";

	protected void Start() {

		Type classType;
		switch (descriptor) {

			case Descriptor.Lyrics:
				classType = typeof(Lyrics);
				break;

		}

		TextReader streamReader = new StreamReader(path);

		//var csv = new CsvReader(streamReader);
		//var records = csv.GetRecords<Lyrics>();

	}

	/////

	public class Lyrics {

		public string webScraperOrder, webScraperStartUrl, artists, artistsHref, songs, songsHref, lyrics;

	}


}
