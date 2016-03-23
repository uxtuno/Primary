Shader "Custom/ColoredObjectShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
			_Metallic("Metallic", Range(0,1)) = 0.0
			_Cutout("Cutout", Range(0,1)) = 0.0
	}
	SubShader{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Opaque" }
			//Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			//Offset -1, -1
			LOD 200

			Pass{
			ColorMask 0
		}

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Lambert alpha// alphatest:_Cutout

			// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput  o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb * _Color.a;
				// Metallic and smoothness come from slider variables
				//o.Metallic = 0;
				//o.Smoothness = 0;
				o.Alpha = _Color.a;
				o.Emission  = _Color.rgb * _Color.a * 0.15;
			}
			ENDCG
		}
		Fallback "Transparent/Diffuse"
}