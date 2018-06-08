using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct P3 : INSortItem {

	public Vector3 p;

	public P3 ( float x, float y, float z ) {

		p = new Vector3 ( x, y, z );

	}


	public float SortingValue {
		get {

			float multiplier = ( p.x + p.y + p.z ) < 0 ? -1f : 1f;
			return p.sqrMagnitude * multiplier;
		
		}
	}



	public string name {
		get {
			return "P3";
		}
		set {
			
		}
	}
}

public class NSortExample : NSortableList {

	public List<P3> myItems;

	public override void Init () {

		myItems = new List<P3> ();

		myItems.Add ( new P3 ( 0, 0, 0 ) );
		myItems.Add ( new P3 ( 1, 0, 0 ) );
		myItems.Add ( new P3 ( 0, 1.5f, 0 ) );
		myItems.Add ( new P3 ( -2f, 1.5f, 0 ) );
		myItems.Add ( new P3 ( 0, -1.5f, 10 ) );

		foreach ( P3 item in myItems ) {

			items.Add ( item );

		}

	}

	protected void Start () {

		Sort ();

		myItems.Clear ();
		int i = 0;
		foreach ( P3 item in items ) {

			Debug.LogFormat ("{0} at {1}", item.p, item.SortingValue );
			myItems.Add ( item );

			LabeledPoint lp = gameObject.AddComponent<LabeledPoint> ();
			lp.Point.transform.position = item.p;
			lp.textMesh.text = string.Format("{0}", i.ToString());
			lp.Point.localScale = Vector3.one * .2f;

			i++;

		}

	}

}
