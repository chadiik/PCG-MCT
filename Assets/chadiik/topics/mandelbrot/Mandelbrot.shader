Shader "chadiik/pcg/Mandelbrot"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Area ("Area", vector) = (0, 0, 4, 4)
		_Angle ("Angle", range(-3.1415, 3.1415)) = 0
		_YMul ("YMul", float) = 2
		_MJRatio ("MJRatio", float) = 0
	}
	SubShader
	{

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		// No culling or depth
		Cull Off ZWrite Off

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
			
			sampler2D _MainTex;
			float4 _Area;
			float _Angle;
			float _YMul;
			float _MJRatio;

			float2 rotate(float2 p, float2 pivot, float a){
				float s = sin(a);
				float c = cos(a);

				p -= pivot;
				p = float2(p.x*c - p.y*s, p.x*s + p.y*c);
				p += pivot;

				return p;
			}

			float mandelbrot(float2 z, float2 c, float iterMax, float limit){

				float x = z.x;
				float y = z.y;
				limit *= limit;

				float iter;
				for(iter = 0; iter < iterMax; iter++){
					x = z.x*z.x - z.y*z.y + c.x;
					y = 2 * z.x*z.y + c.y;
					if( x*x + y*y > limit ) break;
					z.x = x;
					z.y = y;
				}

				return iter / iterMax;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 c = _Area.xy + (i.uv-.5) * _Area.zw;
				c = rotate(c, _Area.xy, _Angle);

				float2 z = float2(3, 2) * (i.uv-.5) * _MJRatio;
				float m = mandelbrot(z, c, 32, 2);
				
				float2 mUV = float2(_YMul + m, .5);
				fixed3 color = tex2D(_MainTex, mUV);

				return fixed4(color, lerp(.001, 1, m));
			}
			ENDCG
		}
	}
}
