Shader "chadiik/pcg/CASim"
{
	Properties
	{
		//_Convolution("Convolution (cell,adj,diag,DT)", 2D) = "white" {}
	}

	CGINCLUDE

	#include "UnityCustomRenderTexture.cginc"

	//sampler2D _Convolution;

	int Cell(in half2 uv)
	{
		return tex2D(_SelfTexture2D, uv).x > .5 ? 1 : 0;
	}

	half4 frag(v2f_customrendertexture i) : SV_Target
	{
		half2 uv = i.globalTexcoord;

		// pixel size
		half2 texel = half2(1. / _CustomRenderTextureWidth, 1. / _CustomRenderTextureHeight);

		int cell = Cell(uv);

		// can be fixed2
		// adjacent cells
		int left = Cell(uv - half2(texel.x, 0.));
		int right = Cell(uv + half2(texel.x, 0.));
		int up = Cell(uv - half2(0., texel.y));
		int down = Cell(uv + half2(0., texel.y));
		// diagonal cells
		int leftUp = Cell(uv - texel);
		int rightDown = Cell(uv + texel);
		int rightUp = Cell(uv + half2(texel.x, -texel.y));
		int leftDown = Cell(uv + half2(-texel.x, texel.y));

		/*
		int k = Cell(px + ivec2(-1, -1)) + Cell(px + ivec2(0, -1)) + Cell(px + ivec2(1, -1))
			+ Cell(px + ivec2(-1, 0)) + Cell(px + ivec2(1, 0))
			+ Cell(px + ivec2(-1, 1)) + Cell(px + ivec2(0, 1)) + Cell(px + ivec2(1, 1));

		int e = Cell(px);

		float f = (((k == 2) && (e == 1)) || (k == 3)) ? 1.0 : 0.0;
		*/
		int k = left + right + up + down
			+ leftUp + rightDown + rightUp + leftDown;

		half f = (((k == 2) && (cell == 1)) || (k == 3)) ? 1.0 : 0.0;

		half3 color = half3(f, f, f);

		return half4(color.r, color.g, color.b, 1.0);
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
