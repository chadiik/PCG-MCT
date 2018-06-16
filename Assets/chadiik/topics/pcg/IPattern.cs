﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPattern : IEnumerable<Vector3> {

	int Count { get; }

}
