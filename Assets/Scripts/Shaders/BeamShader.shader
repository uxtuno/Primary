Shader "Custom/BeamShader" {
	Properties {
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Blend SrcColor OneMinusSrcColor
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		half4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
		
			o.Emission = _Color;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
