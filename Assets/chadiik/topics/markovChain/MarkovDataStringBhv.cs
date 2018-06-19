using MarkovSharp.TokenisationStrategies;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarkovDataStringBhv : MonoBehaviour {

	public List<DataPhrase> input = new List<DataPhrase> ();

	public CSVData csvInput;

	public int n = 1;
	public string start = string.Empty;
	public int sequenceLength = 20;
	public TextMesh textDisplay;

	public DataStringMarkov model;

	public Rand rand;

	protected void Start () {

		//csvInput.Test(2, new[] { "songs", "lyrics" });
		csvInput.Test();

		model = new DataStringMarkov(n);
		model.EnsureUniqueWalk = true;

		//string[] lineDelim = new[] { "\r\n", "\r", "\n" };
		string[] wordDelim = new[] { " " };
		char[] trimChars = new[] { '\r', '\n', ' ', '"', '\'' };

		if ( csvInput != null ) {

			List<string> lyrics = csvInput.Data[ "lyrics" ];
			List<string> songsInfo = csvInput.Data[ "songs" ];

			textDisplay.text = "";

			int numLyrics = lyrics.Count;
			Debug.LogFormat ( "Lyrics #: {0}", numLyrics );
			for ( int iLyric = 0; iLyric < numLyrics; iLyric++ ) {

				string info = songsInfo[ iLyric ];
				string[] words = lyrics[ iLyric ].Trim ().Split ( wordDelim, System.StringSplitOptions.RemoveEmptyEntries );

				int numWords = words.Length;
				DataString[] wordsData = new DataString[ numWords ];

				for(int iWord = 0; iWord < numWords; iWord++) {

					DataString word = new DataString(words[iWord].Trim(trimChars));
					word.data.Add ( "info", info );
					word.data.Add ( "index", iLyric.ToString() );

					wordsData[iWord] = word;

					if(iLyric < 10) {

						textDisplay.text += word.value;

					}

				}

				DataPhrase phrase = new DataPhrase ( wordsData );
				input.Add ( phrase );

				// Train the model
				model.Learn(phrase);

			}

		}

		if ( rand == null ) {

			rand = gameObject.AddComponent<Rand> ();

		}

	}

	protected void Update () {

		if ( Input.GetKeyDown ( KeyCode.Space ) ) {

			GenerateSequence ();

			Debug.LogFormat ( "Sequence: {0}", sequence );

			if ( textDisplay != null ) {

				textDisplay.text = sequence;

				StartCoroutine ( LogTextCoroutine () );

			}

		}

	}

	private IEnumerator LogTextCoroutine () {

		yield return new WaitForFixedUpdate ();
		yield return new WaitForFixedUpdate ();

		Debug.LogFormat ( "Sequence: {0}", textDisplay.text );

	}

	public string sequence;
	private string GenerateSequence () {

		DataString seedContent = string.IsNullOrEmpty(start) ? new DataString(string.Empty) : new DataString(start);
		DataPhrase seed = new DataPhrase(new[] { seedContent });
		//IEnumerable<string> result = model.Walk ( sequenceLength, seed );
		IEnumerable<DataPhrase> result = model.Walk ( sequenceLength, seed, true );
		//IEnumerable<string> result = model.WalkRepeat ( sequenceLength, seed );
		sequence = "";
		string previousLyricIndex = string.Empty;
		foreach ( DataPhrase phrase in result ) {

			sequence += phrase.ToString ( (token) => {

				string index;
				if(token.data.TryGetValue("index", out index)) {

					bool sameSource = previousLyricIndex == index;
					string word = string.Format("{0}{1} ", sameSource ? "" : "|", token.ToString());
					previousLyricIndex = index;

					return word;

				}

				return string.Empty;

			}) + "\n";

		}

		return sequence;

	}

}
