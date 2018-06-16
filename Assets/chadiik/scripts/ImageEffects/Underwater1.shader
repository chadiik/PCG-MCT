Shader "chadiik/image/Underwater1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Slope("Slope", float) = 1.0
		_A("A", float) = 5.0
		_B("B", float) = 7.0
		_Rate("Rate", float) = 1.0
		_Intensity("Intensity", float) = 0.5
		_Frequency("Frequency", float) = 1
		_LightColor("LightColor", Color) = (1, 1, 1, 1)
		_FillColor("FillColor", Color) = (0, 0, 1, 0.5)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			uniform sampler2D _MainTex;
			uniform float _Slope;
			uniform float _A;
			uniform float _B;
			uniform float _Rate;
			uniform float _Intensity;
			uniform float _Frequency;
			uniform float4 _LightColor;
			uniform float4 _FillColor;

			fixed4 frag (v2f i) : SV_Target
			{
				
				float slope = _Slope;
				float sa = _A * slope;
				float sb = _B * slope;

				float time = _Time.y;
				float f = _Frequency;

				float2 uv = i.uv;
				float bright =
					-sin(uv.y * sa + uv.x * 30.0 * f + time * 3.10 * f) *.2
					- sin(uv.y * sb + uv.x * 37.0 * f + time * 3.10 * f) *.1
					- cos(+uv.x * 2.0 * slope + time * 2.10 * f) *.1
					- sin(-uv.x * 5.0 * slope + time * 2.0 * f) * .3;

				float modulate = abs(cos(time * .1 * _Rate) *.5 + sin(time * .7 * _Rate)) * _Intensity;
				bright *= modulate;
				
				fixed4 tex = tex2D(_MainTex, uv);
				tex.rgb = lerp(tex.rgb + clamp(bright / 1.0,0.0,1.0) * _LightColor.rgb, _FillColor.rgb, _FillColor.a);
				
				return tex;
			}
			ENDCG
		}
	}
}
