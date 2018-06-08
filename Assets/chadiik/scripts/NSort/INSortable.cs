using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INSortItem {

	float SortingValue { get; }
	string name { get; set; }

}

public interface INSortable : IEnumerable<INSortItem> {


	
}
