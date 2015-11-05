Shader "KingKong/Effect/UVAnimation"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent+1"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Name "BASE"

			Blend One One
			Fog{ Mode Off }
			Lighting Off
			ZWrite Off
			Offset -1, -1

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			fixed4 _MainTex_ST;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy + _Time.x * _MainTex_ST.zw;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);

				return c;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}