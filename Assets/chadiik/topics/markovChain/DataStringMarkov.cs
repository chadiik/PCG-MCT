using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MarkovSharp.Models;

namespace MarkovSharp.TokenisationStrategies {

	public class DataString : IComparable {

		//public static DataString Default = new DataString ( "#" );
		
		public string value;
		public Dictionary<string, string> data;

		public DataString ( string value ) {

			this.value = value;
			data = new Dictionary<string, string> ();

		}

		public override string ToString () {

			return value;

		}

		public int CompareTo ( object obj ) {

			if ( obj == null ) return 1;

			DataString x = obj as DataString;

			if ( x != null )
				return this.value.CompareTo ( x.value );
			else
				throw new ArgumentException ( "Object is not a DataString" );

		}

		public override bool Equals ( object o ) {
			DataString x = o as DataString;
			if ( x == null ) {
				return false;
			}

			bool equals = this.value == x.value;

			return equals;
		}

		public override int GetHashCode () {
			unchecked {
				int hash = this.value.GetHashCode ();
				//hash = hash * 23 + data.GetHashCode ();
				return hash;
			}
		}
	}

	public class DataPhrase {

		public IEnumerable<DataString> tokens;

		public DataPhrase ( IEnumerable<DataString> tokens ) {

			this.tokens = tokens;

		}

		public override string ToString () {

			string result = "";
			foreach ( DataString token in tokens ) {

				if ( token != null )
					result += token.ToString () + " ";

			}

			return result;

		}

		public string ToString(System.Func<DataString, string> toString = null) {
			
			string result = "";
			foreach (DataString token in tokens) {

				if (token != null)
					result += toString.Invoke( token ) + " ";

			}

			return result;

		}

	}

	public class DataStringMarkov : GenericMarkov<DataPhrase, DataString> {
		public DataStringMarkov ( int level = 2 )
			: base ( level ) { }

		public override IEnumerable<DataString> SplitTokens ( DataPhrase input ) {
			if ( input == null ) {
				return new List<DataString> { GetPrepadUnigram () };
			}
			
			return input.tokens;
		}

		public override DataPhrase RebuildPhrase ( IEnumerable<DataString> tokens ) {
			//UnityEngine.Debug.LogFormat ( "RebuildPhrase(tokens: {0})", tokens.ArrayToString ( item => { return ArabicSupport.ArabicFixer.Fix ( item.ToString (), true, true ); } ) );

			DataPhrase phrase = new DataPhrase( tokens );

			return phrase;
		}

		public override DataString GetTerminatorUnigram () {
			return null;
		}

		public override DataString GetPrepadUnigram () {
			//return DataString.Default;
			return new DataString ( "" );
		}

		public override ChainPhraseProbability<DataPhrase> GetFit(DataPhrase testData) {
			return base.GetFit(testData);


		}

		/*public override double GetTransitionProbabilityUnigram(DataPhrase currentState, DataString nextState) {
			try {

				UnityEngine.Debug.LogFormat("override GetTransitionProbabilityUnigram");


				List <DataString> potentialNext = GetMatches(currentState);
				double probability = (double)potentialNext.Count(s => s.Equals(nextState)) / potentialNext.Count;

				double scramble = 1;

				if (probability > .0001) {

					DataString last = currentState.tokens.Last();
					string currentIndex, nextIndex;
					if (last != null && nextState != null && last.data.TryGetValue("index", out currentIndex)) {

						if(nextState.data.TryGetValue("index", out nextIndex) && currentIndex != nextIndex) {

							scramble = 0;
							UnityEngine.Debug.LogFormat("Scrambling {0}, {1}-{2}", last, currentIndex, nextIndex);

						}

					}

				}

				return probability * scramble;
			}
			catch (KeyNotFoundException) {
				return 0;
			}
		}*/
	}
}
