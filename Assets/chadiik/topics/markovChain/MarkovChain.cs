using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkovChain {

	public int n;
	public Dictionary<int, Gram> grams;

	public MarkovChain ( int n ) {

		this.n = n;

		grams = new Dictionary<int, Gram> ();

	}

	public void Learn ( List<IState> states ) {

		int numStates = states.Count;
		int maxGramState = numStates - 1 - n;

		for ( int iState = 0; iState < maxGramState; iState++ ) {

			Gram gramStates = new Gram ( n, states.GetRange ( iState, n ) );

			if ( grams.ContainsKey ( gramStates.uid ) == false ) {

				grams.Add ( gramStates.uid, gramStates );

			}

			if ( iState < maxGramState - n )
				gramStates.AddLink ( new Gram ( n, states.GetRange ( iState + n, n ) ) );

		}

	}

	public virtual List<IState> Sequence ( List<IState> seed, int length, Rand rand ) {

		int seedKey = new Gram ( n, seed ).uid;

		List<IState> result = new List<IState> ( length );
		result.AddRange ( seed );

		while ( result.Count < length ) {

			Gram current;
			if ( grams.TryGetValue ( seedKey, out current ) == false ) {

				break;

			}

			Gram next = current.Next ( rand.Float () );
			Debug.LogFormat ( "{0} -> {1}", current, next );
			result.Add ( next.Last );

			seedKey = next.uid;

		}

		return result;

	}

	public override string ToString () {

		return string.Format ( "MarkovChain[{0}] {1}", n, grams.ArrayToString () );

	}

	/////

	public class Gram : IState {

		public int n;
		public List<IState> grams;
		public List<Gram> links;

		public Gram ( int n, List<IState> grams ) {

			this.n = n;
			this.grams = grams;

		}

		public Gram ( int n, IState gram ) {

			this.n = n;
			this.grams = new List<IState> ();
			this.grams.Add ( gram );

		}

		public void Add ( IState state ) {

			grams.Add ( state );

		}

		public void AddLink ( Gram state ) {

			if ( links == null ) {

				links = new List<Gram> ();

			}

			links.Add ( state );

		}

		public Gram Next ( float r ) {

			if ( links == null ) {

				return this;

			}

			int index = Mathf.FloorToInt ( r * ( float )links.Count );
			Gram next = links[ index ];

			return next;

		}

		public IState Last {

			get {

				return grams[ grams.Count - 1 ];

			}

		}

		public override string ToString () {

			string gramStr = "";
			foreach ( IState state in grams ) {
				gramStr += state.ToString ();
			}

			string linkStr = "Null";
			if ( links != null ) {
				foreach ( Gram gram in links ) {
					linkStr += gram.ToString ();
				}
			}

			string result = gramStr;

			return result;

		}


		public int uid {
			get {

				int uid = 0;

				foreach ( IState gram in grams ) {

					uid += gram.uid;
					
				}

				return uid;
			
			}
		}
	}

	public interface IState {

		int uid { get; }

	}

}
