// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/AlphaMask" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Alpha("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Lighting Off

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _Alpha;
	float4 _MainTex_ST;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 col = tex2D(_MainTex, i.texcoord);
	fixed4 col2 = tex2D(_Alpha, i.texcoord);
	col.a = col2.a + 0.0001;
	return col;
	}
		ENDCG
	}
	}
}