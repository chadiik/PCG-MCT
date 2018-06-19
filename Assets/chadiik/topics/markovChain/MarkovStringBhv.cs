using MarkovSharp.TokenisationStrategies;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarkovStringBhv : MonoBehaviour {

	public List<string> input = new List<string>
	{
		"Frankly, my dear, I don't give a damn.",
		"Mama always said life was like a box of chocolates. You never know what you're gonna get.",
		"Many wealthy people are little more than janitors of their possessions."
	};

	public CSVData csvInput;

	public int n = 1;
	public string start = string.Empty;
	public int sequenceLength = 20;
	public TextMesh textDisplay;

	public StringMarkov model;

	public Rand rand;

	protected void Start () {

		if ( csvInput != null ) {

			List<string> lyrics = csvInput.Data[ "lyrics" ];

			if ( true ) {

				input.Clear ();

				string[] lineDelim = new[] { "\r\n", "\r", "\n" };
				foreach ( string lyric in lyrics ) {

					string[] lines = lyric.Split ( lineDelim, System.StringSplitOptions.None );

					for ( int i = 0; i < lines.Length - 1; i += 2 )
						input.Add ( lines[ i ] + " " + lines[ i + 1 ] );

				}

			}/*
			else {

				input = lyrics;

			}*/

		}

		model = new StringMarkov ( n );
		model.EnsureUniqueWalk = true;

		// Train the model
		model.Learn ( input );

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

		string seed = string.IsNullOrEmpty ( start ) ? null : start;
		//IEnumerable<string> result = model.Walk ( sequenceLength, seed );
		IEnumerable<string> result = model.Walk ( sequenceLength, seed, true );
		//IEnumerable<string> result = model.WalkRepeat ( sequenceLength, seed );
		sequence = string.Join ( "\n", result );

		return sequence;

	}

}
