using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IEnumerableExtensions {

	public static string ArrayToString ( this IEnumerable array, System.Func<object, string> toString = null ) {

		if ( array == null ) return "Null";

		System.Text.StringBuilder result = new System.Text.StringBuilder ();

		int count = 0;

		foreach ( object item in array ) {

			result.AppendFormat ( "{0}, ", item != null ? ( toString == null ? item.ToString () : toString.Invoke ( item ) ) : "Null" );
			count++;

		}

		string prefix = array.GetType ().Name;
		if ( prefix.IndexOf ( "[]" ) != -1 ) {

			prefix = prefix.Replace ( "[]", string.Format ( " [ {0} ] ", count ) );

		}
		else {

			prefix += " [ " + count + " ] ";

		}

		if ( count > 0 ) {

			result.Insert ( 0, string.Format ( "{0}{{ ", prefix ) );
			result.Remove ( result.Length - 2, 1 );
			result.Append ( "}" );

			return result.ToString ();

		}

		return prefix;

	}

}

public class WebCamTexBhv : MonoBehaviour {

	public WebCamTexture webcamTexture;

	public Material filterMaterial;
	public RenderTexture filteredTexture;

	public Material material;

	protected void Start () {

		webcamTexture = new WebCamTexture ();
		Debug.Log ( WebCamTexture.devices.ArrayToString ( ( object item ) => {

			WebCamDevice device = ( WebCamDevice )item;
			return device.name;

		}));

		webcamTexture.Play ();

		if ( material == null ) material = GetComponent<Renderer> ().sharedMaterial;

		if ( filterMaterial != null ) {

			filteredTexture = new RenderTexture ( webcamTexture.width, webcamTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
			material.mainTexture = filteredTexture;

		}
		else {

			material.mainTexture = webcamTexture;

		}

	}

	protected void FixedUpdate () {

		if ( filteredTexture != null && webcamTexture.isPlaying ) {

			Graphics.Blit ( webcamTexture, filteredTexture, filterMaterial );

		}

	}

}
