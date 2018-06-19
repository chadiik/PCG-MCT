using DigitalRubyShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCamera : MonoBehaviour {

	// Fingers
	#region fingers

	private TapGestureRecognizer tapGesture;
	private TapGestureRecognizer doubleTapGesture;
	private TapGestureRecognizer tripleTapGesture;
	private SwipeGestureRecognizer swipeGesture;
	private PanGestureRecognizer panGesture;
	private ScaleGestureRecognizer scaleGesture;
	private RotateGestureRecognizer rotateGesture;
	private LongPressGestureRecognizer longPressGesture;

	private void InitFingers () {

		CreateTapGesture ();
		CreatePanGesture ();
		CreateScaleGesture ();

	}

	private void UpdateFingers () {

	}

	private bool m_TapBegan = false;
	private void TapGestureCallback ( GestureRecognizer gesture ) {
		if ( gesture.State == GestureRecognizerState.Began ) {
			m_TapBegan = true;
			OnMouseDownHandler ( gesture.FocusX, gesture.FocusY );
		}
		else if ( gesture.State == GestureRecognizerState.Ended ) {
			m_TapBegan = false;
			OnMouseUpHandler ( gesture.FocusX, gesture.FocusY );
		}
	}

	private void CreateTapGesture () {
		tapGesture = new TapGestureRecognizer ();
		tapGesture.StateUpdated += TapGestureCallback;
		//tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
		FingersScript.Instance.AddGesture ( tapGesture );
	}

	private void PanGestureCallback ( GestureRecognizer gesture ) {
		if ( gesture.State == GestureRecognizerState.Executing ) {

			OnMouseDragHandler ( panGesture.DeltaX, panGesture.DeltaY );
		}
	}

	private void CreatePanGesture () {
		panGesture = new PanGestureRecognizer ();
		panGesture.MinimumNumberOfTouchesToTrack = 2;
		panGesture.StateUpdated += PanGestureCallback;
		FingersScript.Instance.AddGesture ( panGesture );
	}

	private void ScaleGestureCallback ( GestureRecognizer gesture ) {
		if ( gesture.State == GestureRecognizerState.Executing ) {
			OnPinchHandler( scaleGesture.ScaleMultiplier );
		}
	}

	private void CreateScaleGesture () {
		scaleGesture = new ScaleGestureRecognizer ();
		scaleGesture.StateUpdated += ScaleGestureCallback;
		FingersScript.Instance.AddGesture ( scaleGesture );
	}

	#endregion

	// Handlers

	private Vector2 ToViewport ( float pixelX, float pixelY ) {
		return new Vector2 ( pixelX / Screen.width, pixelY / Screen.height );
	}

	private void OnMouseDownHandler ( float x, float y ) {
		
	}

	private void OnMouseUpHandler ( float x, float y ) {
		
	}

	private void OnMouseDragHandler ( float distanceX, float distanceY ) {
		UpdateDrag ( ToViewport ( distanceX, distanceY ) );
	}

	private void OnPinchHandler ( float scaleMultiplier ) {
		UpdateZoom ( scaleMultiplier );
	}

	// Camera

	public OrbitController controller;

	private void UpdateDrag ( Vector2 viewportDelta ) {

		controller.deltaMove = -viewportDelta;

	}

	private void UpdateZoom ( float zoom ) {

		controller.zoom = zoom - 1f;

	}

	// MonoBehaviour

	protected void Start () {

		InitFingers ();

	}

	protected void Update () {

		UpdateFingers ();

	}

}
