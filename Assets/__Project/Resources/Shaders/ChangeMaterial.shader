Shader "CrossFade"
{
	Properties
	{
		_Blend("Blend", Range(0, 1)) = 0.5
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_TexMat1("Texture 1", 2D) = "white" {}
	_TexMat2("Texture 2", 2D) = ""
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 300
		Pass
	{
		SetTexture[_TexMat1]
		SetTexture[_TexMat2]
	{
		ConstantColor(0, 0, 0,[_Blend])
		Combine texture Lerp(constant) previous
	}
	}

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _TexMat1;
	sampler2D _TexMat2;
	fixed4 _Color;
	float _Blend;

	struct Input
	{
		float2 uv_TexMat1;
		float2 uv_TexMat2;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed4 t1 = tex2D(_TexMat1, IN.uv_TexMat1) * _Color;
		fixed4 t2 = tex2D(_TexMat2, IN.uv_TexMat1) * _Color;
		o.Albedo = lerp(t1, t2, _Blend);
	}
	ENDCG
	}
		FallBack "Diffuse"
}