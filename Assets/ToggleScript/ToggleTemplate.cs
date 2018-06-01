using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class ToggleTemplate : ScriptableObject {

	public string path;

	[Header("Defines")]
	public string define;
	public BuildTargetGroup[] platforms = new BuildTargetGroup[] { BuildTargetGroup.Standalone };

	public bool enabled = true;

}
