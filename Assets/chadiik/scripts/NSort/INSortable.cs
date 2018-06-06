using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INSortItem {

	float SortingValue { get; }

}

public interface INSortable : IEnumerable<INSortItem> {


	
}
