Shader "Custom/alpha1" {

	Properties{
		_MainTex("Color (RGB)", 2D) = "white"
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		CGPROGRAM
#pragma surface surf NoLighting alpha 

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}

	struct Input {
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	void surf(Input IN, inout SurfaceOutput o) {
		o.Emission = tex2D(_MainTex, IN.uv_MainTex).rgb;
		o.Alpha = 1;
	}

	ENDCG
	}

}