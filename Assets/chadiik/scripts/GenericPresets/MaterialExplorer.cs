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
		
	}

	protected void Update () {

		if ( update ) {

			update = false;
			Explore ( null );

		}

		if(preset != null ) {

			if ( properties == null ) {
				FromPreset ( preset );
			}

			preset.ApplyMaterial ( material );

			preset = null;

		}

	}

	public void FromPreset( GenericPreset source ) {

		properties = new List<GenericPreset.Property> ();

		foreach ( GenericPreset.Property property in source.properties ) {

			properties.Add ( property.Clone () );

		}

		Debug.LogFormat ( "FromPreset {0}", properties.ArrayToString () );

	}

	public List<GenericPreset.Property> Explore ( GenericPreset template ) {

		Shader shader = material.shader;

		if ( Application.isEditor && template == null ) {
#if UNITY_EDITOR
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

			return properties;

#endif
		}

		if ( properties == null && template == null )
			return null;

		if( properties == null || properties.Count != template.properties.Count )
			FromPreset ( template );

		foreach ( GenericPreset.Property property in properties ) {

			string propertyName = property.key;

			switch ( property.type ) {

				case GenericPreset.PType.Number:
					property.Value = material.GetFloat ( propertyName );
					break;

				case GenericPreset.PType.V3:
					property.Value = material.GetVector ( propertyName );
					break;

				case GenericPreset.PType.Color:
					property.Value = material.GetColor ( propertyName );
					break;

			}

		}

		return properties;

	}


}
