using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSortableList : MonoBehaviour, INSortable {

	public bool needsUpdate = true;

	public delegate int SortFunction<in INSortItem>(INSortItem a, INSortItem b);

	protected void Awake () {

		needsUpdate = true;

	}

	public virtual void Init () {

	}

	public void Sort ( SortFunction<INSortItem> sortFunction = null ) {

		if ( needsUpdate ) {

			Init ();
			needsUpdate = false;

		}

		if ( sortFunction == null ) sortFunction = SortBySortingValue;
		items.Sort ( new Comparison<INSortItem> ( sortFunction ) );

	}

	public static int SortRandom ( INSortItem a, INSortItem b ) {

		return ( int )( Rand.Instance.Float () * 4f - 2f );

	}
	
	public static int SortBySortingValueInverted(INSortItem a, INSortItem b){

		float d = a.SortingValue - b.SortingValue;
		if ( d < 0 ) return 1;
		if ( d > 0 ) return -1;
		return 0;

	}

	public static int SortBySortingValue ( INSortItem a, INSortItem b ) {

		float d = a.SortingValue - b.SortingValue;
		if ( d < 0 ) return -1;
		if ( d > 0 ) return 1;
		return 0;

	}

	// IEnumerable

	public List<INSortItem> items;

	public virtual IEnumerator<INSortItem> GetEnumerator () {

		if ( needsUpdate ) {

			Init ();
			needsUpdate = false;

		}

		for ( int i = 0, numItems = items.Count; i < numItems; i++ ) {

			yield return items[ i ];

		}

	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {

		return GetEnumerator ();

	}

	public override string ToString () {

		string result = "NSortableList(items = [";

		for ( int i = 0, numItems = items.Count; i < numItems; i++ ) {

			string name = items[i].name;
			result += name.Substring ( 0, Mathf.Min ( name.Length, 6 ) ) + ( i < numItems - 1 ? ", " : "" );

		}

		result += "]";

		result += ")";

		return result;

	}

}
