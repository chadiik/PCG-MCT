using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ( menuName = "PCG/Reaction Diffusion/Preset" )]
public class RDPreset : ScriptableObject {

	public float convCell = -1, convAdj = .2f, convDiag = .05f;
	public float aRate = 1f, bRate = .5f;
	public float feedRate = .055f, killRate = .062f;
	public float deltaTime = 1;

	public float convolutionSum = 0f;

	public string ID {
		
		get {

			const string nf = ".####";
			string aggregate = string.Format ( "RD({0}x{1}x{2}_{3}x{4}_{5}x{6}_{7})", convCell.ToString(nf), convAdj.ToString(nf), convDiag.ToString(nf), aRate.ToString(nf), bRate.ToString(nf), feedRate.ToString(nf), killRate.ToString(nf), deltaTime.ToString(nf) )
				.Replace('.', 'p').Replace('-', 'm');

			return aggregate;

		}

	}

	private float _convCell, _convAdj, _convDiag, _aRate, _bRate, _feedRate, _killRate, _deltaTime;
	public bool hasChanged {

		get {

			bool changed = _convCell != convCell || _convAdj != convAdj || _convDiag != convDiag || _aRate != aRate || _bRate != bRate || _feedRate != feedRate || _killRate != killRate || _deltaTime != deltaTime;

			if ( changed ) {

				_convCell = convCell; _convAdj = convAdj; _convDiag = convDiag;

				convolutionSum = convCell + convAdj * 4 + convDiag * 4;

				_aRate = aRate; _bRate = bRate;
				_feedRate = feedRate; _killRate = killRate;
				_deltaTime = deltaTime;

			}

			return changed;

		}

	}

	public RDPreset Copy ( RDPreset target = null ) {

		if ( target == null ) target = ScriptableObject.CreateInstance<RDPreset> ();

		target.convCell = convCell;
		target.convAdj = convAdj;
		target.convDiag = convDiag;

		target.aRate = aRate;
		target.bRate = bRate;

		target.feedRate = feedRate;
		target.killRate = killRate;

		target.deltaTime = deltaTime;

		return target;

	}

}

