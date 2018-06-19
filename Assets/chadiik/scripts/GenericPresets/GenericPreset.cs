using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "PCG/Generic/Preset" )]
public class GenericPreset : ScriptableObject {

	public enum PType { Null, Number, V3, Color };

	[ExecuteInEditMode]
	[System.Serializable]
	public class Property {
		public string key;
		public PType type;
		public float[] values;

		public float Float { get { return values [ 0 ]; } }
		public Vector3 V3 { get { return new Vector3 ( values [ 0 ], values [ 1 ], values [ 2 ] ); } }
		public Color Color { get { return new Color ( values [ 0 ], values [ 1 ], values [ 2 ], values [ 3 ] ); } }

		public object Value {
			set {
				switch ( type ) {
					case PType.Number:
						values [ 0 ] = (float)value;
						break;

					case PType.V3:
						Vector3 v = (Vector3) value;
						values [ 0 ] = v.x;
						values [ 1 ] = v.y;
						values [ 2 ] = v.z;
						break;

					case PType.Color:
						Color c = (Color)value;
						values [ 0 ] = c.r;
						values [ 1 ] = c.g;
						values [ 2 ] = c.b;
						values [ 3 ] = c.a;
						break;
				}
			}
			get {
				switch ( type ) {
					case PType.Number:
						return Float;

					case PType.V3:
						return V3;

					case PType.Color:
						return Color;
				}

				return null;
			}
		}

		public Property(string key, PType type ) {
			this.key = key;
			this.type = type;
			switch ( type ) {
				case PType.Number:
					values = new float [ 1 ];
					break;
				case PType.V3:
					values = new float [ 3 ];
					break;
				case PType.Color:
					values = new float [ 4 ];
					break;
			}
		}

		public Property(string key, float value ) {
			this.key = key;
			type = PType.Number;
			values = new [] { value };
		}

		public Property ( string key, Vector3 value ) {
			this.key = key;
			type = PType.V3;
			values = new [] { value.x, value.y, value.z };
		}

		public Property ( string key, Color value ) {
			this.key = key;
			type = PType.Color;
			values = new [] { value.r, value.g, value.b, value.a };
		}

		public Property Clone () {

			Property clone = new Property(key, type);
			clone.Value = Value;

			return clone;

		}

		public override string ToString () {
			return string.Format ( "Property({0}: {1})", key, Value.ToString () );
		}

		public static object Lerp(Property a, Property b, float t ) {

			PType type = a.type;
			if(type != b.type ) {
				Debug.LogFormat ( "Property.Lerp type mismatch: {0} / {1}", type.ToString (), b.type.ToString () );
				return null;
			}

			switch ( type ) {
				case PType.Number:
					return Mathf.Lerp ( a.Float, b.Float, t );

				case PType.V3:
					return Vector3.Lerp ( a.V3, b.V3, t );

				case PType.Color:
					return Color.Lerp ( a.Color, b.Color, t );
			}

			return null;

		}

		public static Property Get ( string key, object property, Type type ) {

			if ( type == ( 0f ).GetType () || type == ( 0 ).GetType () )
				return new Property ( key, ( float ) property );
			else if ( type == typeof ( Vector3 ) )
				return new Property ( key, ( Vector3 ) property );
			else if ( type == typeof ( Color ) )
				return new Property ( key, ( Color ) property );

			Debug.LogWarningFormat ( "Property parse error: {0}, {1}, {2}", key, property, type.ToString () );
			return null;

		}
	}

	[SerializeField]
	public List<Property> properties;

	public Property Find ( string key ) {

		foreach ( Property prop in properties ) {

			if ( prop.key == key )
				return prop;

		}

		return null;

	}

	public void Feed ( UnityEngine.Object source, string[] propertyNames ) {

		if ( properties == null )
			properties = new List<Property> ( propertyNames.Length );

		Type sourceType = source.GetType();

		foreach ( string property in propertyNames ) {

			System.Reflection.FieldInfo sourceField = sourceType.GetField ( property );
			Type propertyType = null;
			object sourceProperty = null;
			if (sourceField != null ) {

				sourceProperty = sourceField.GetValue ( source );
				propertyType = sourceField.FieldType;

			}
			else {

				System.Reflection.PropertyInfo sourcePropertyInfo = sourceType.GetProperty ( property );
				sourceProperty = sourcePropertyInfo.GetValue ( source );
				propertyType = sourcePropertyInfo.PropertyType;

			}

			Property prop = Find( property );

			if ( prop == null ) {

				prop = Property.Get ( property, sourceProperty, propertyType );
				properties.Add ( prop );

			}
			else {

				prop.Value = sourceProperty;

			}

		}

	}

	public void UpdateProperties ( UnityEngine.Object source ) {

		Type sourceType = source.GetType();

		foreach ( Property property in properties ) {

			System.Reflection.FieldInfo sourceField = sourceType.GetField ( property.key );

			object sourceProperty = null;
			if ( sourceField != null ) {

				sourceProperty = sourceField.GetValue ( source );

			}
			else {

				System.Reflection.PropertyInfo sourcePropertyInfo = sourceType.GetProperty ( property.key );
				sourceProperty = sourcePropertyInfo.GetValue ( source );

			}

			property.Value = sourceProperty;

		}

	}

	public void UpdateProperties ( IEnumerable<Property> newProperties ) {

		foreach ( Property newProperty in newProperties ) {

			Property property = Find(newProperty.key);

			if(property != null)
				property.Value = newProperty.Value;

		}

	}

	public void Apply ( UnityEngine.Object target ) {

		if ( properties == null )
			return;

		Type targetType = target.GetType();

		foreach ( Property property in properties ) {

			System.Reflection.FieldInfo targetField = targetType.GetField ( property.key );

			if ( targetField != null ) {

				targetField.SetValue ( target, property.Value );

			}
			else {

				System.Reflection.PropertyInfo targetProperty = targetType.GetProperty ( property.key );
				targetProperty.SetValue ( target, property.Value );

			}

		}
	}

	public void ApplyMaterial ( Material material ) {

		if ( properties == null )
			return;

		foreach ( Property property in properties ) {

			switch ( property.type ) {

				case PType.Number:
					material.SetFloat ( property.key, property.Float );
					break;

				case PType.V3:
					material.SetVector ( property.key, property.V3 );
					break;

				case PType.Color:
					material.SetColor ( property.key, property.Color );
					break;

			}

		}

	}

	public static GenericPreset Create( UnityEngine.Object source, string[] propertyNames ) {

		GenericPreset newPreset = ScriptableObject.CreateInstance<GenericPreset>();
		newPreset.Feed ( source, propertyNames );
		return newPreset;

	}

	public static GenericPreset Create ( List<Property> properties ) {

		GenericPreset newPreset = ScriptableObject.CreateInstance<GenericPreset>();
		if ( newPreset.properties == null )
			newPreset.properties = new List<Property> ( properties.Count );

		foreach ( Property property in properties ) {

			if ( property.type != PType.Null )
				newPreset.properties.Add ( property.Clone () );

		}

		return newPreset;

	}

	public static void Lerp ( GenericPreset a, GenericPreset b, float t, GenericPreset store ) {

		if ( store.properties == null )
			store.properties = new List<Property> ();

		foreach ( Property propA in a.properties ) {

			Property propB = b.Find(propA.key);
			if ( propB != null ) {

				Property propStore = store.Find(propA.key);
				if ( propStore == null ) {

					propStore = new Property ( propA.key, propA.type );
					store.properties.Add ( propStore );

				}

				propStore.Value = Property.Lerp ( propA, propB, t );

			}

		}

	}

}
