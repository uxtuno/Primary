Shader "Custom/ColorBlockShader" {
	Properties{
		_Color("Base (RGBA)", Color) = (1.0, 0.0, 0.0, 0.5)
		_MainTex("Tecture", 2D) = "White"
		_Velocity("Velocity", float) = 1.0
	}
		SubShader{
				Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }
				LOD 200

		Blend SrcAlpha OneMinusSrcAlpha 	// アルファを使用する.

			CGPROGRAM

#pragma surface surf Lambert alpha

		//half4 LightingSimpleLambert(SurfaceOutput s, half3 lightDir, half atten) {
		//	float NdotL = dot(s.Normal, lightDir);
		//	half diff = NdotL;
		//	half4 c;
		//	c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 1)/* * _LightColor0.rgb * (diff * atten * 2)*/;
		//	c.a = s.Alpha;
		//	return c;
		//}

		sampler2D _MainTex;
		half _Velocity;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			//half3 col2 = tex2D(_MainTex, IN.uv_MainTex).rgb * (_Color.rgb / (1 - _Velocity));
			half3 col2 = (_Color.rgb * _Velocity) + tex2D(_MainTex, IN.uv_MainTex).rgb * (1 - _Velocity);
			half4 col = half4(col2.r, col2.g, col2.b, _Color.a);
			o.Albedo = dot(col.rgb, float3(0.3, 0.59, 0.11)) * ((1 - col.a) / 10.0) + (col.rgb * col.a);
			o.Alpha = col.a;
			o.Emission = col.rgb / 50;
		}

		ENDCG

	}
		Fallback "Transparent/Diffuse"
}