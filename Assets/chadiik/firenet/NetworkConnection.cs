using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkConnection : NetworkManager {

	private static NetworkConnection m_Instance;
	public static NetworkConnection Instance {
		get {
			
			if (m_Instance == null) {

				m_Instance = GameObject.FindObjectOfType<NetworkConnection> ();

			}

			return m_Instance;
		}
	}

	public static int PORT = 7777;

	private NetworkClient m_Client;
	private bool m_IsHost;

	public delegate void ConnectionEvent ( int connectionId );
	public ConnectionEvent OnClientHasConnected;

	protected void Start () {



	}

	public override void OnServerConnect ( UnityEngine.Networking.NetworkConnection connection ) {

		if ( m_Client != null && m_Client.connection.connectionId != connection.connectionId ) {

			if ( OnClientHasConnected != null ) {

				OnClientHasConnected ( connection.connectionId );

			}

		}

	}

	public void StartHost () {

		m_IsHost = true;

		SetPort ( PORT );
		m_Client = NetworkManager.singleton.StartHost ();
		string networkAddress = Network.player.ipAddress;
		Debug.Log ( networkAddress );

		
	}

	public void StartClient ( string ipAddress ) {

		SetIPAddress ( ipAddress );
		SetPort ( PORT );
		NetworkManager.singleton.StartClient ();

	}

	private void SetPort ( int port ) {

		NetworkManager.singleton.networkPort = port;

	}

	private void SetIPAddress ( string ipAddress ) {

		NetworkManager.singleton.networkAddress = ipAddress;

	}

	public static string GetMACAddress () {

		string firstMacAddress = System.Net.NetworkInformation.NetworkInterface
			.GetAllNetworkInterfaces ()
			.Where ( nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback )
			.Select ( nic => nic.GetPhysicalAddress ().ToString () )
			.FirstOrDefault ();

		return firstMacAddress;

	}

}
