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

	float4 _Convolution;
	float _A;
	float _B;
	float _Feed;
	float _Kill;
	float _DT;

	float4 frag(v2f_customrendertexture i) : SV_Target
	{
		float2 uv = i.globalTexcoord;

		// pixel size
		float2 texel = float2(1. / _CustomRenderTextureWidth, 1. / _CustomRenderTextureHeight);

		float4 cell = tex2D(_SelfTexture2D, uv);

		// 3x3 convolution:
		float cellWeight = _Convolution.x;
		float adjacentWeight = _Convolution.y;
		float diagonalWeight = _Convolution.z;
		
		// can be fixed2
		// adjacent cells
		float4 left = tex2D(_SelfTexture2D, uv - float2(texel.x, 0.));
		float4 right = tex2D(_SelfTexture2D, uv + float2(texel.x, 0.));
		float4 up = tex2D(_SelfTexture2D, uv - float2(0., texel.y));
		float4 down = tex2D(_SelfTexture2D, uv + float2(0., texel.y));
		// diagonal cells
		float4 leftUp = tex2D(_SelfTexture2D, uv - texel);
		float4 rightDown = tex2D(_SelfTexture2D, uv + texel);
		float4 rightUp = tex2D(_SelfTexture2D, uv + float2(texel.x, -texel.y));
		float4 leftDown = tex2D(_SelfTexture2D, uv + float2(-texel.x, texel.y));

		// convolution result
		float4 convolution = 
			cell * cellWeight + 
			(left + right + up + down) * adjacentWeight + 
			(leftUp + leftDown + rightUp + rightDown) * diagonalWeight;

		// http://www.karlsims.com/rd.html
		// Reaction (A) in red/x
		float a = cell.x;
		// Reaction (B) in green/y
		float b = cell.y;

		float abb = a * b * b;
		
		float rdA =
			a +
			( 
				_A * convolution.x - abb +
				_Feed * (1. - a)
			) * _DT;

		float rdB =
			b +
			(
				_B * convolution.y + abb -
				(_Kill + _Feed) * b
			) * _DT;

		float4 color = float4(saturate(rdA), saturate(rdB), 0., 0.);

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
