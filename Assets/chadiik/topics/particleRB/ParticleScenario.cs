#if FIREBASE
using Firebase.Database;
#else
using FirebaseInterface;
using FirebaseInterface.Database;
#endif

using pcg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScenario : MonoBehaviour {

	private struct ParticleData {
		public string key;
		public float x, y;
		public override string ToString () {

			return "ParticleData(" + x.ToString ( ".00" ) + ", " + y.ToString ( ".00" ) + ")";

		}

		public static ParticleData FromSnapshot ( DataSnapshot snapshot ) {

			float x = float.Parse(snapshot.Child("x").Value.ToString()),
				y = float.Parse(snapshot.Child("y").Value.ToString());

			ParticleData particle = new ParticleData{x=x, y=y, key=snapshot.Key};
			return particle;

		}
	}

	public Transform orbitTemplate;
	public AffectedParticleRB particle;

	protected void Start () {

		if ( FirebaseManager.Instance != null ) {

			FirebaseManager.Instance.SetValueAsync ( "pcg/topic", "particles" );
			FirebaseManager.Instance.Path ( "pcg/particles" ).ChildAdded += FB_OnParticleAdded;

		}

	}

	private void FB_OnParticleAdded ( object sender, ChildChangedEventArgs args ) {

		ParticleData particle = ParticleData.FromSnapshot(args.Snapshot);
		Debug.Log ( args.Snapshot.Key + ":" + particle );

		Vector3 screenPoint = new Vector3 ( particle.x * (float)Screen.width, (1 - particle.y) * (float)Screen.height, 0 );
		InstantiateOrbitAtScreenPoint ( screenPoint );

	}

	protected void Update () {

		if( Input.GetMouseButtonDown( 0 ) ) {

			InstantiateOrbitAtScreenPoint ( Input.mousePosition );

		}

	}

	private void InstantiateOrbitAtScreenPoint ( Vector3 screenPoint ) {

		Ray ray = Camera.main.ScreenPointToRay ( screenPoint );
		float intersects;
		( new Plane ( Vector3.forward, Vector3.zero ) ).Raycast ( ray, out intersects );

		if ( intersects > 0 ) {

			Vector3 position = Camera.main.transform.position + ray.direction * intersects;
			InstantiateOrbit ( position );

		}

	}

	private void InstantiateOrbit ( Vector3 position ) {

		Transform orbitPoint = Instantiate ( orbitTemplate );
		orbitPoint.transform.position = position;

		ParticleAffectorRB[] affectors = orbitPoint.GetComponentsInChildren<ParticleAffectorRB> ();

		foreach ( ParticleAffectorRB affector in affectors ) {

			affector.life = 8;
			particle.affectors.Add ( affector );
			
		}

	}

}
