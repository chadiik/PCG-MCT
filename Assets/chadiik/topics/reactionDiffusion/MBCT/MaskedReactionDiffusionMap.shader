Shader "chadiik/pcg/MaskedReactionDiffusionMap"
{
	Properties
	{
		_Convolution ("Convolution (cell,adj,diag,DT)", 2D) =  "white" {}
		_Rates ("Rates (A,B,Feed,Kill)", 2D) =  "white" {} // (0.6702115, 0.2730684, 0.08909346, 0.06200349) lines wiggly
		_Timevar ("Time (A,B,Feed,Kill)", vector) = (0, 0, 0, 0)
	}

	CGINCLUDE

    #include "UnityCustomRenderTexture.cginc"

	sampler2D _Convolution;
	sampler2D _Rates;
	half4 _Timevar;

	half4 frag(v2f_customrendertexture i) : SV_Target
	{
		half2 uv = i.globalTexcoord;

		float4 convolutionSample = tex2D(_Convolution, uv);
		float _DT = convolutionSample.w;

		float4 ratesSample = tex2D(_Rates, uv);
		float _A = ratesSample.x;
		float _B = ratesSample.y;
		float _Feed = ratesSample.z;
		float _Kill = ratesSample.w;

		// pixel size
		half2 texel = half2(1. / _CustomRenderTextureWidth, 1. / _CustomRenderTextureHeight);

		half4 cell = tex2D(_SelfTexture2D, uv);

		// 3x3 convolution:
		half cellWeight = convolutionSample.x;
		half adjacentWeight = convolutionSample.y;
		half diagonalWeight = convolutionSample.z;
		
		// can be fixed2
		// adjacent cells
		half4 left = tex2D(_SelfTexture2D, uv - half2(texel.x, 0.));
		half4 right = tex2D(_SelfTexture2D, uv + half2(texel.x, 0.));
		half4 up = tex2D(_SelfTexture2D, uv - half2(0., texel.y));
		half4 down = tex2D(_SelfTexture2D, uv + half2(0., texel.y));
		// diagonal cells
		half4 leftUp = tex2D(_SelfTexture2D, uv - texel);
		half4 rightDown = tex2D(_SelfTexture2D, uv + texel);
		half4 rightUp = tex2D(_SelfTexture2D, uv + half2(texel.x, -texel.y));
		half4 leftDown = tex2D(_SelfTexture2D, uv + half2(-texel.x, texel.y));

		// convolution result
		half4 convolution = 
			cell * cellWeight + 
			(left + right + up + down) * adjacentWeight + 
			(leftUp + leftDown + rightUp + rightDown) * diagonalWeight;

		// http://www.karlsims.com/rd.html
		// Reaction (A) in red/x
		half a = cell.x;
		// Reaction (B) in green/y
		half b = cell.y;

		// time

		_A += _Timevar.x;
		_B += _Timevar.y;
		_Feed += _Timevar.z;
		_Kill += _Timevar.w;

		half abb = a * b * b;
		
		half rdA =
			a +
			( 
				_A * convolution.x - abb +
				_Feed * (1. - a)
			) * _DT;

		half rdB =
			b +
			(
				_B * convolution.y + abb -
				(_Kill + _Feed) * b
			) * _DT;

		half4 color = half4(saturate(rdA), saturate(rdB), 0., 0.);

		return color;
	}

	ENDCG

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			Name "Update"
			CGPROGRAM
			#pragma vertex CustomRenderTextureVertexShader
			#pragma fragment frag
			ENDCG
		}
	}

}
