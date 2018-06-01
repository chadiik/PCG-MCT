using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pcg {

	public struct LSSymbol {

		public int id;

		public LSSymbol ( string character ) {

			id = character[ 0 ];

		}

		public LSSymbol ( char character ) {

			id = character;

		}

		public override bool Equals ( object obj ) {

			return obj is LSSymbol && ((LSSymbol)obj).id == id;

		}

		public static bool operator == ( LSSymbol symbol1, LSSymbol symbol2 ) {

			return symbol1.Equals ( symbol2 );

		}

		public static bool operator != ( LSSymbol symbol1, LSSymbol symbol2 ) {

			return ! symbol1.Equals ( symbol2 );

		}

		public override string ToString () {

			return ( ( char )id ).ToString ();

		}

	}

	public class LSSeed {

		public List<LSSymbol> symbols;

		public LSSeed () {

		}

		public LSSeed ( string seed ) {

			int len = seed.Length;

			this.symbols = new List<LSSymbol> ( len );

			for ( int i = 0; i < len; i++ ) {

				this.symbols.Add( new LSSymbol ( seed[ i ] ) );

			}

		}

		public override string ToString () {

			string result = "LSSeed(";

			foreach ( LSSymbol symbol in symbols ) {

				result += symbol.ToString ();
				
			}

			result += ")";

			return result;

		}

	}

	public class LSRule {

		public LSSymbol input;
		public LSSymbol[] output;

		public LSRule(LSSymbol input, LSSymbol[] output){

			this.input = input;
			this.output = output;

		}

		public static LSRule FromString(string input, string output){

			LSSymbol inputSb = new LSSymbol(input);
			
			int numOutput = output.Length;
			LSSymbol[] outputSbs = new LSSymbol[numOutput];

			for(int i = 0; i < numOutput; i++){

				outputSbs[i] = new LSSymbol(output[i]);

			}

			LSRule rule = new LSRule(inputSb, outputSbs);

			return rule;

		}

		public static LSRule FromString ( string rule ) {

			string[] components = rule.Split ( '=' );

			string input = components[ 0 ];
			string output = components[ 1 ];

			return FromString ( input, output );

		}

	}

	public class LSystem : MonoBehaviour {

		public string seedInput;
		
		[Tooltip("Symbol=Symbols")]
		public string[] rulesInput;

		public string constantsInput;

		public LSSeed seed;
		public LSRule[] rules;
		public LSSymbol[] constants;

		protected void Start () {

			if ( seed == null ) {

				seed = new LSSeed ( seedInput );

			}

			if ( rules == null ) {

				int numRules = rulesInput.Length;
				rules = new LSRule[ numRules ];

				for ( int i = 0; i < numRules; i++ ) {

					rules[ i ] = LSRule.FromString ( rulesInput[ i ] );

				}

			}

			if ( constants == null ) {

				int numConstants = constantsInput.Length;
				constants = new LSSymbol[ numConstants ];

				for ( int i = 0; i < numConstants; i++ ) {

					constants[ i ] = new LSSymbol ( constantsInput[ i ] );

				}

			}

		}

		public void Pass () {

			List<LSSymbol> newSymbols = new List<LSSymbol>(); // Make double buffered seed (2 symbols Lists)

			foreach ( LSSymbol seedSymbol in seed.symbols ) {

				foreach ( LSRule rule in rules ) {

					if ( seedSymbol == rule.input ) {

						newSymbols.AddRange ( rule.output );

					}

				}

				foreach ( LSSymbol constant in constants ) {

					if ( seedSymbol == constant ) {

						newSymbols.Add ( seedSymbol );

					}

				}
				
			}

			seed.symbols = newSymbols;

			Debug.LogFormat ( "Pass: {0}", seed );

		}

		protected void Update () {

			if ( Input.GetMouseButtonDown ( 0 ) ) {

				Pass ();

			}

		}


	}

}