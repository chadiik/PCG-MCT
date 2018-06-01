//#define DG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pcg;

public class PatternTurtle : Pattern {

	public const int ANTICLOCK  = '+';
	public const int CLOCKWISE  = '-';
	public const int PUSH       = '[';
	public const int POP        = ']';
	public const int COLOR     = 'C';
	public const int CONSTANT     = '!';

	public LSystem lSystem;
	public float angle = 90, distance = 1;

	private Bounds bounds;
	private int maxStackDepth;

	public override void Init () {

		if ( lSystem.seed != null )
			UpdatePath ( lSystem.seed );

	}

	public void UpdatePath ( LSSeed seed ) {

		Draw ( seed.symbols );

	}

	struct Location {

		public Vector3 position;
		public Quaternion heading;
		public int colorIndex;

		public Location ( float x, float y, float angle, int colorIndex ) {

			position = new Vector3 ( x, y, 0 );
			heading = Quaternion.AngleAxis ( angle, Vector3.forward );

			this.colorIndex = colorIndex;

		}

		public void AddAngle ( float angle ) {

			Vector3 euler = heading.eulerAngles;
			euler.z += angle;

			heading = Quaternion.Euler ( euler );

		}

	}

	public void Draw ( List<LSSymbol> cmds ) {

		Stack<Location> stack = new Stack<Location> ();
		float lastX, lastY;

		// start at grid 0,0 facing north with no color index
		Location pos = new Location ( 0.0f, 0.0f, 90.0f, 0 );

		// process each command in turn
		float yOffset = 0;//this._yOffset, 
		Color[] colorList = new Color[] { Color.black, Color.black, Color.black, Color.black, Color.black };

		bool draw = true;

		Vector3 v3;

		float rad;
		float width;
		Color color;
		Color lastcolor = Color.black;
		int len = cmds.Count;

		#if DG
		string log = "";
#endif

		for ( int i=0; i < len; i++ ) {

			LSSymbol c = cmds[ i ];

			switch ( c.id ) {
				// ANTICLOCK  = '+'
				// CLOCKWISE  = '-'
				// PUSH       = '['
				// POP        = ']'
				// COLOR      = 'C'

				case COLOR:
					// get color index from next character
					pos.colorIndex = cmds[ ++i ].id;

					#if DG
					log += " / color " + pos.colorIndex;
#endif

					break;

				case ANTICLOCK:
					pos.AddAngle ( angle );

					#if DG
					log += " / turn + " + pos.heading.eulerAngles;
#endif

					break;

				case CLOCKWISE:
					pos.AddAngle ( -angle );

					#if DG
					log += " / turn - " + pos.heading.eulerAngles;
#endif

					break;

				case PUSH:
					stack.Push ( pos );

					#if DG
					log += " / PUSH";
#endif

					break;

				case POP:
					pos = stack.Pop ();
					v3 = new Vector3 ( pos.position.x, pos.position.y + yOffset, 0 );
					vectors.Add ( v3 );

					#if DG
					log += " / POP";
#endif

					break;

				case CONSTANT:
					
					break;

				default:
					#if DG
					log += " / Line ";
#endif

					lastX = pos.position.x;
					lastY = pos.position.y;

					// move the turtle
					rad = pos.heading.eulerAngles.z * Mathf.PI / 180f;
					pos.position.x += distance * Mathf.Cos ( rad );
					pos.position.y += distance * Mathf.Sin ( rad );

					if ( draw ) {

						// render this element
						/*if (renderLineWidths)
						{
						   width = (maxStackDepth - stack.length);
						   ctx.lineWidth = width >= 1 ? width : 1;
						}*/
						color = colorList[ pos.colorIndex ];
						if ( lastcolor != color ) {
							//ctx.strokeStyle = color;
							lastcolor = color;
						}

						//ctx.beginPath();
						/* ctx.moveTo(lastX, HEIGHT - (lastY + yOffset));
						 ctx.lineTo(pos.x, HEIGHT - (pos.y + yOffset));*/

						v3 = new Vector3( pos.position.x, pos.position.y + yOffset, 0 );
						vectors.Add ( v3 );

						//ctx.closePath();
						//ctx.stroke();

					}
					else {

						// remember min/max position
						Vector3 min = Vector3.one * float.MinValue;
						Vector3 max = Vector3.one * float.MaxValue;
						if ( pos.position.x < min.x ) min.x = pos.position.x;
						else if ( pos.position.x > max.x ) max.x = pos.position.x;
						if ( pos.position.y < min.y ) min.y = pos.position.y;
						else if ( pos.position.y > max.y ) max.y = pos.position.y;

						Vector3 extent = max - min;
						bounds = new Bounds ( Vector3.zero, extent );

						int numStack = stack.Count;
						if ( numStack > maxStackDepth ) maxStackDepth = numStack;

					}

					break;
			}

		}

		#if DG
		DG.Log ( log );
#endif

	}

	protected void Update () {

		if ( Input.GetMouseButtonDown ( 0 ) ) {

			Preview ();

		}

	}

	private Transform preview;
	private void Preview () {

		m_Initiated = false;

		if ( preview != null ) {

			DestroyImmediate ( preview.gameObject );

		}

		preview = new GameObject ().transform;
		preview.parent = transform;

		StartCoroutine ( PreviewCoroutine () );

	}

	private IEnumerator PreviewCoroutine () {

		float duration = Mathf.Min ( .1f, 3f / (float)vectors.Count );

		foreach ( Vector3 v3 in this ) {

			Transform p = GameObject.CreatePrimitive ( PrimitiveType.Cube ).transform;
			p.parent = preview;
			p.position = v3;

			StartCoroutine ( HighlightedFor ( p, .5f, .2f, .2f ) );

			yield return new WaitForSeconds ( Mathf.Min ( .1f, 3f / ( float )vectors.Count ) );

		}

	}

	private IEnumerator HighlightedFor ( Transform target, float highlightScale, float normalScale, float duration ) {

		target.localScale = Vector3.one * highlightScale;

		yield return new WaitForSeconds ( duration );

		if(target != null)
			target.localScale = Vector3.one * normalScale;

	}

}
