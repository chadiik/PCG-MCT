using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FIREBASE

#else

using System.Threading.Tasks;
using FirebaseInterface.Database;

namespace FirebaseInterface {

	public class DatabaseReference {

		public DatabaseReference Child ( object value ) { return new DatabaseReference (); }
		public object Value;
		public string Key;

		public delegate void FirebaseEvent ( object sender, FirebaseInterface.Database.ChildChangedEventArgs args );
		public FirebaseEvent ChildAdded;

		public Task SetValueAsync ( object value ) {
			return new Task ( DummyAction );
		}

		public void DummyAction () {

		}

		public Task<DataSnapshot> GetValueAsync () {
			return new Task<DataSnapshot> ( DummySnapshotAction );
		}

		private DataSnapshot dummySnapshot = new DataSnapshot ();
		public DataSnapshot DummySnapshotAction () {

			return dummySnapshot;

		}

	}

	public class FirebaseApp {

		public static FirebaseApp DefaultInstance;
		public void SetEditorDatabaseUrl ( object value ) { }

	}

	public class FirebaseDatabase {

		public static FirebaseDatabase DefaultInstance;
		public DatabaseReference RootReference;

	}

	namespace Database {

		public class DataSnapshot : DatabaseReference {
		}

		public class ChildChangedEventArgs {

			public DataSnapshot Snapshot;

		}

	}

}



#endif