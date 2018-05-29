Shader "chadiik/image/Line" {
	
	Properties {

		_MainTex ("Base (RGB)", 2D) = "white" {}
		_A ("Point A", Vector) = (0, 0, 0)
		_B ("Point B", Vector) = (0, 0, 0)
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Thickness ("Thickness", Float) = 100.

	}

	SubShader {

		Cull Off ZWrite Off ZTest Always

		Pass {

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float3 _A;
			uniform float3 _B;
			uniform float4 _Color;
			uniform float _Thickness;

			float solveLine( in float2 p, in float2 a, in float2 b, float blur, float thickness){

				// blur = 0.9985, size = 100.0
				float2 pa = p - a;
				float2 ba = b - a;
				float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
				float d = length( pa - ba*h );
	
				//float sharp = smoothstep(1.0/iResolution.y, 0., d );
				return clamp(((1.0 - d)-blur)*thickness, 0.0, 1.0);

			}

			float4 frag(v2f_img i) : COLOR {

				float4 main = tex2D(_MainTex, i.uv);
				
				float lineResult = solveLine(i.uv, _A, _B, .9985, _Thickness);
				float4 result = main + lineResult * _Color;
				return result;

			}

			ENDCG

		}

	}

}
