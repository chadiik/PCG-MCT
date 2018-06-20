Shader "chadiik/pcg/CASeed"
{
	Properties
	{
		_Seed("Toggle", Float) = 0
		_Frequency("Frequency", Float) = .01
	}

		CGINCLUDE

#include "UnityCustomRenderTexture.cginc"

		//sampler2D _Convolution;

		float _Seed;
	float _Frequency;

	float hash1(float n)
	{
		return frac(sin(n)*138.5453123);
	}

	half4 frag(v2f_customrendertexture i) : SV_Target
	{

		float f = 0;
		if (_Seed > 0.00001) {

			half2 uv = i.globalTexcoord;
			f = hash1( _Seed + uv.x * 13.0 + hash1( uv.y * 71.1) ) < _Frequency ? 1.0 : 0.0;

		}

		return half4(f, 0, 0, 1.0);
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
