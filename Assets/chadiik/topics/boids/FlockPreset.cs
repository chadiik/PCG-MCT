using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "PCG/Boids/Preset" )]
public class FlockPreset : ScriptableObject {

	public float separationWeight = 1;
	public float alignmentWeight = 1;
	public float cohesionWeight = 1;
	public float headToOriginWeight = 1;
	public float speed = 1.0f;
	public float currentHeadingWeight = 1.0f;
	public float rotationSmooth = .1f;
	public float nearRadius = 10f;

}
