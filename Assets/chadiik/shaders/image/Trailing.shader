Shader "chadiik/image/Trailing" {
	
	Properties {

		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OldTex ("Base (RGB)", 2D) = "white" {}
		_Blend ("Main/Old Blend", Range (0, 1)) = 0

	}

	SubShader {

		Cull Off ZWrite Off ZTest Always

		Pass {

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _OldTex;
			uniform float _Blend;

			float4 frag(v2f_img i) : COLOR {

				float4 main = tex2D(_MainTex, i.uv);
				float4 old = tex2D(_OldTex, i.uv);
				
				float4 result = main + old * _Blend;
				return result;

			}

			ENDCG

		}

	}

}
