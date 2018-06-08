Shader "chadiik/pcg/MaskedRDSurface"
{
	Properties
	{
		_MainTex("RD Texture", 2D) = "white" {}
		_Threshold("Threshold", Range(0, 1)) = 0.1
		_EdgeSmooth("Edge Smoothing", Range(0, 1)) = 0.2
		_NormalStrength("Normal Strength", Range(0, 1)) = 0.9
		[Space]
		_AlbedoTex("Albedo texture", 2D) = "white" {}
		_Color0("Color 0", Color) = (1,1,1,1)
		_Color1("Color 1", Color) = (1,1,1,1)
		[Space]
		_Smoothness0("Smoothness 0", Range(0, 1)) = 0.5
		_Smoothness1("Smoothness 1", Range(0, 1)) = 0.5
		[Space]
		_Metallic0("Metallic 0", Range(0, 1)) = 0.0
		_Metallic1("Metallic 1", Range(0, 1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		CGPROGRAM

		#pragma surface surf Standard
		#pragma target 3.0

		struct Input { float2 uv_MainTex; };

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		half _Threshold, _EdgeSmooth;
		sampler2D _AlbedoTex;
		fixed4 _Color0, _Color1;
		half _Smoothness0, _Smoothness1;
		half _Metallic0, _Metallic1;
		half _NormalStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float3 texel = float3(_MainTex_TexelSize.xy, 0);

			half v0 = tex2D(_MainTex, IN.uv_MainTex).y;
			half v1 = tex2D(_MainTex, IN.uv_MainTex - texel.xz).y;
			half v2 = tex2D(_MainTex, IN.uv_MainTex + texel.xz).y;
			half v3 = tex2D(_MainTex, IN.uv_MainTex - texel.zy).y;
			half v4 = tex2D(_MainTex, IN.uv_MainTex + texel.zy).y;

			half v00 = (v0 + v1 + v2 + v3 + v4) / 5.;
			half p = smoothstep(_Threshold, _Threshold + _EdgeSmooth, v0);

			float3 colorLerp = lerp(_Color0.rgb, _Color1.rgb, p);
			float3 albedoTexLerp = tex2D(_AlbedoTex, IN.uv_MainTex).rgb * p;
			o.Albedo = colorLerp * .666 + albedoTexLerp * .333;
			//o.Albedo = colorLerp;
			o.Smoothness = lerp(_Smoothness0, _Smoothness1, p);
			o.Metallic = lerp(_Metallic0, _Metallic1, p);
			o.Normal = normalize(float3(v1 - v2, v3 - v4, 1 - _NormalStrength));
		}

		ENDCG
	}
	FallBack "Diffuse"
}
