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

			items = new List<INSortItem> ();
			Init ();
			needsUpdate = false;

		}

		if ( sortFunction == null ) sortFunction = SortBySortingValue;
		items.Sort ( new Comparison<INSortItem> ( sortFunction ) );

	}
	
	public static int SortBySortingValue(INSortItem a, INSortItem b){

		float d = a.SortingValue - b.SortingValue;
		if ( d < 0 ) return -1;
		if ( d > 0 ) return 1;
		return 0;

	}

	// IEnumerable

	public List<INSortItem> items;

	public virtual IEnumerator<INSortItem> GetEnumerator () {

		if ( needsUpdate ) {

			items = new List<INSortItem> ();
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

}
