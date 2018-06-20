Shader "chadiik/pcg/CASeedR"
{
	Properties
	{
		_Seed("Toggle", Float) = 0
		_Frequency("Frequency", Float) = .01
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float _Seed;
			float _Frequency;

			float hash1(float n)
			{
				return frac(sin(n)*138.5453123);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float f = 0;
				if (_Seed > 0.00001) {
					f = hash1(_Seed + i.uv.x * 13.0 + hash1(i.uv.y * 71.1)) < _Frequency ? 1.0 : 0.0;
				}

				return fixed4(f, 0.0, 0.0, 1.0);
			}
			ENDCG
		}
	}
}
