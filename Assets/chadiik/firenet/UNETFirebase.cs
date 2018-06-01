using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UNETFirebase : MonoBehaviour {

	public bool isHost;
	public string MACFilter;

	protected void Start () {

		#if FIREBASE
		if( !string.IsNullOrEmpty( MACFilter ) && MACFilter[0] != '!' ) {

			isHost = 
				MACFilter == NetworkConnection.GetMACAddress ();

		}

		Setup ( isHost );
#endif

	}

	private void Setup ( bool isHost ) {

		NetworkConnection unet = NetworkConnection.Instance;
		FirebaseManager firebase = FirebaseManager.Instance;

		firebase.ExecuteOnInitialisation (
			() => {

				string networkAddress;

				if (isHost) {

					networkAddress = Network.player.ipAddress;
					firebase.SetValueAsync ( "connection/ip", networkAddress );
					firebase.SetValueAsync ( "connection/run", UnityEngine.Random.Range ( 0, 100 ) );

				}
				else {

					firebase.GetValueAsync("connection/ip",
						( value ) => {

							networkAddress = ( string )value;
							unet.StartClient ( networkAddress );

						}
					);

				}

			}
		);

		if ( isHost ) {

			unet.StartHost ();

			unet.OnClientHasConnected +=
				( connectionId ) => {

					Debug.Log ( "Client " + connectionId + " Connected!" );

				}
			;

		}

	}


}
