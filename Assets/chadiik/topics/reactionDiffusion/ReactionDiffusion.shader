Shader "chadiik/pcg/ReactionDiffusion"
{
	Properties
	{
		_Convolution ("Convolution (cell,adj,diag,0)", vector) = (-1., .2, .05, 0)

		_A ("A Diffusion Rate", float) = 0
		_B ("B Diffusion Rate", float) = 0
		_Feed ("Feed Rate", float) = 0
		_Kill ("Kill Rate", float) = 0

		_DT ("Delta Time", float) = 1
	}

	CGINCLUDE

    #include "UnityCustomRenderTexture.cginc"

	half4 _Convolution;
	half _A;
	half _B;
	half _Feed;
	half _Kill;
	half _DT;

	half4 frag(v2f_customrendertexture i) : SV_Target
	{
		half2 uv = i.globalTexcoord;

		// pixel size
		half2 texel = half2(1. / _CustomRenderTextureWidth, 1. / _CustomRenderTextureHeight);

		half4 cell = tex2D(_SelfTexture2D, uv);

		// 3x3 convolution:
		half cellWeight = _Convolution.x;
		half adjacentWeight = _Convolution.y;
		half diagonalWeight = _Convolution.z;
		
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
