using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour {

	private static OrbitController c_Instance;
	public static OrbitController Instance {
		get {
			if ( c_Instance == null ) c_Instance = GameObject.FindObjectOfType<OrbitController> ();
			return c_Instance;
		}
	}

	public Transform orbitTarget;

	[Space]

	public float deltaMoveMultiplier = 1f;
	public float zoomMultipler = 1f;

	[Header("Damping")]
	public float positionalDamping = 1f;
	public float rotationalDamping = 1f;

	private Vector3 m_TargetPosition;
	private Quaternion m_TargetRotation;
	public Vector3 deltaMove = Vector3.zero;
	public float zoom = 0f;

	private void DeltaMove () {

		Vector2 deltaViewport = deltaMove * deltaMoveMultiplier;

		Vector3 direction = transform.TransformDirection( deltaViewport );
		m_TargetPosition = m_TargetPosition + direction;

		deltaMove = Vector3.zero;

	}

	private void Zoom () {

		Vector3 offset = transform.forward * zoom * zoomMultipler;
		m_TargetPosition = m_TargetPosition + offset;

		zoom = 0f;

	}

	public Vector3 TargetPosition{
		set {
			m_TargetPosition = value;
		}
		get {
			return m_TargetPosition;
		}
	}

	public Quaternion TargetRotation {
		set {
			m_TargetRotation = value;
		}
		get {
			return m_TargetRotation;
		}
	}

	// MonoBehaviour

	protected void Start () {

		m_TargetPosition = transform.position;

	}

	protected void Update () {

		DeltaMove ();
		Zoom ();

		Vector3 newPosition = Vector3.Lerp(transform.position, m_TargetPosition, positionalDamping);
		transform.position = newPosition;

		if(orbitTarget != null ) {
			m_TargetRotation = Quaternion.LookRotation ( ( orbitTarget.position - newPosition ).normalized );
		}

		Quaternion newRotation = Quaternion.Lerp(transform.rotation, m_TargetRotation, rotationalDamping);
		transform.rotation = newRotation;

	}

	public void EditModeUpdate () {
		transform.position = TargetPosition;
		if ( orbitTarget != null )
			TargetRotation = Quaternion.LookRotation ( ( orbitTarget.position - TargetPosition ).normalized );

		transform.rotation = TargetRotation;
	}

	internal void Apply ( CameraControllerPreset camView ) {

		TargetPosition = camView.position;
		TargetRotation = Quaternion.Euler ( camView.eulerAngles );

	}
}
