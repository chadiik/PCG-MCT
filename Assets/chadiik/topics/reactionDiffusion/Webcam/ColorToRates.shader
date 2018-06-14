Shader "chadiik/utils/ColorToRates"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Rates ("Rates", 2D) = "white" {}
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
			sampler2D _Rates;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 color = tex2D(_MainTex, i.uv);

				float u = length(color.rgb) / 3.;
				u = color.r; // rates should be ordered by fill capacity (dark areas = less fill)
				fixed4 rate = tex2D(_Rates, float2(u, 0));

				return rate;
			}
			ENDCG
		}
	}
}
