using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUNET : NetworkBehaviour{

	protected void Start (){

		if ( isLocalPlayer == false ){
			// Is a different Network client object

			return;

		}

		CmdSpawn ();

	}


	///// SERVER COMMANDS
	[Command]
	void CmdSpawn () {

		// Spawn on all clients
		//GameObject someTemplateGameObject = ...
		//NetworkServer.Spawn( someTemplateGameObject ); // SpawnWithClientAuthority( go, ctc );
		//someTemplateGameObject.GetComponent<NetworkIdentity>().AssignClientAuthority( connectionToClient );

	}


	///// REMOTE PROCEDURE CALL
	[ClientRpc]
	void RpcToThisObjectOnAnotherClient ( string n ) {

		Debug.Log ( n );

	}


	///// SyncVar
	[SyncVar]
	public string syncedVar = "var0";

}
