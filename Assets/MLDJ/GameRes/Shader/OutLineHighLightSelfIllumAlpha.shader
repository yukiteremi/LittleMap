Shader "Outline_/OutLineHighLightSelfIllumAlpha" {
	Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_Illum ("Illumin (A)", 2D) = "white" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
	_Cutoff ("Alpha cutoff",Float) = 0.5
	_whiteColor("WhiteColor",Float)=0.0
}

SubShader {
	Tags {"Queue" = "Transparent-10" "IgnoreProjector"="True"}
	LOD 200
	
CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff

sampler2D _MainTex;
sampler2D _Illum;
fixed4 _Color;
float _whiteColor;
struct Input {
	float2 uv_MainTex;
	float2 uv_Illum;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	fixed4 c1=fixed4(1.0f,1.0f,1.0f,1.0f);
	if(_whiteColor>0.1f)
	{
	  o.Albedo=c1;
	  o.Alpha = c.a;
	  o.Emission=1.0f;
	}
	else
	{
	o.Albedo = c.rgb;
	o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
	o.Alpha = c.a;
	}
	
}
ENDCG
} 

FallBack "Transparent/VertexLit"
}
