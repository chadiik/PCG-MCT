using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MaterialExplorer : MonoBehaviour {

	public Material material;
	public List<GenericPreset.Property> properties;
	public GenericPreset preset;
	public bool update = false;

	protected void Start () {

		Explore ();

	}

	protected void Update () {

		if ( update ) {

			update = false;
			Explore ();

		}

		if(preset != null ) {

			preset.ApplyMaterial ( material );
			preset = null;

		}

	}

	public void Explore () {

		Shader shader = material.shader;
		int numProperties = ShaderUtil.GetPropertyCount( shader );

		if ( properties == null )
			properties = new List<GenericPreset.Property> ( numProperties );
		else
			properties.Clear ();

		for ( int i = 0; i < numProperties; i++ ) {

			string propertyName = ShaderUtil.GetPropertyName(shader, i);
			ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);

			GenericPreset.Property property = new GenericPreset.Property ( propertyName, GenericPreset.PType.Null);

			switch ( type ) {
				case ShaderUtil.ShaderPropertyType.Float:
				case ShaderUtil.ShaderPropertyType.Range:
					property = new GenericPreset.Property ( propertyName, material.GetFloat ( propertyName ) );
					break;

				case ShaderUtil.ShaderPropertyType.Vector:
					property = new GenericPreset.Property ( propertyName, ( Vector3 ) material.GetVector ( propertyName ) );
					break;

				case ShaderUtil.ShaderPropertyType.Color:
					property = new GenericPreset.Property ( propertyName, material.GetColor ( propertyName ) );
					break;
			}

			properties.Add ( property );

		}

	}


}
