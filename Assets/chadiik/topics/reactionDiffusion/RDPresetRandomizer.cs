using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PropValueKey {

	public string prop;
	public object value;

	public PropValueKey ( string prop, object value ) {
		this.prop = prop;
		this.value = value;
	}

}

[CreateAssetMenu ( menuName = "PCG/Reaction Diffusion/Randomizer Preset" )]
public class RDPresetRandomizer : ScriptableObject {

	[System.Serializable]
	public struct RG {

		public float value, min, max;

		public RG ( float value = .5f, float min = 0, float max = 1 ) {

			this.value = value;
			this.min = min;
			this.max = max;

		}

		public void Fuzzy () {

			this.value = Rand.Instance.Float ( min, max );

		}

	}

	public RG convCell = new RG ( -1 ), convAdj = new RG ( .2f ), convDiag = new RG ( .05f );
	public RG aRate = new RG ( 1f ), bRate = new RG ( .5f );
	public RG feedRate = new RG ( .055f ), killRate = new RG ( .062f );
	public RG deltaTime = new RG ( 1 );

	string[] propertyNames = new string[] { "convCell", "convAdj", "convDiag", "aRate", "bRate", "feedRate", "killRate", "deltaTime" };

	public void Apply ( RDPreset preset ) {

		Type targetType = typeof ( RDPreset );
		Type ownType = typeof ( RDPresetRandomizer );

		foreach ( string property in propertyNames ) {

			System.Reflection.FieldInfo targetField = targetType.GetField ( property );
			System.Reflection.FieldInfo ownField = ownType.GetField ( property );

			RG range = ( RG )ownField.GetValue ( this );

			// Randomize
			range.Fuzzy ();

			targetField.SetValue ( preset, range.value );

		}

	}

	public float Lerp ( string property, float t ) {

		Type ownType = typeof ( RDPresetRandomizer );

		//System.Reflection.FieldInfo targetField = targetType.GetField ( property );
		System.Reflection.FieldInfo ownField = ownType.GetField ( property );

		RG range = ( RG )ownField.GetValue ( this );

		return range.min + t * ( range.max - range.min );

	}

	public float Unlerp ( string property, float t ) {

		Type ownType = typeof ( RDPresetRandomizer );

		System.Reflection.FieldInfo ownField = ownType.GetField ( property );

		RG range = ( RG )ownField.GetValue ( this );

		return ( t - range.min ) / ( range.max - range.min );

	}

	public void InitAroundBase ( RDPreset preset, PropValueKey[] pairs ) {

		Type targetType = typeof ( RDPreset );
		Type ownType = typeof ( RDPresetRandomizer );

		foreach ( PropValueKey pair in pairs ) {

			string property = pair.prop;
			float radius = ( float )pair.value;

			System.Reflection.FieldInfo targetField = targetType.GetField ( property );
			System.Reflection.FieldInfo ownField = ownType.GetField ( property );

			float baseValue = ( float )targetField.GetValue ( preset );

			RG range = new RG ( baseValue, baseValue - radius, baseValue + radius );
			ownField.SetValue ( this, range );

		}

	}

	public void InitAroundBase ( RDPreset preset, float radius ) {

		Type targetType = typeof ( RDPreset );
		Type ownType = typeof ( RDPresetRandomizer );

		foreach ( string property in propertyNames ) {

			System.Reflection.FieldInfo targetField = targetType.GetField ( property );
			System.Reflection.FieldInfo ownField = ownType.GetField ( property );

			float baseValue = ( float )targetField.GetValue ( preset );

			RG range = new RG ( baseValue, baseValue - radius, baseValue + radius );
			ownField.SetValue ( this, range );

		}

	}

}