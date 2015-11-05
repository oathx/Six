Shader "KingKong/Effect/Display"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
		_InterlacePattern("InterlacePattern", 2D) = "black" {}
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Lighting Off
			Fog{ Mode Off }

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			uniform	fixed4 _MainTex_ST;
			sampler2D _InterlacePattern;
			fixed4 _InterlacePattern_ST;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed2 uv2 : TEXCOORD1;
			};

			v2f vert(a2v v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
				o.uv2.xy = TRANSFORM_TEX(v.texcoord.xy, _InterlacePattern) + _Time.x * _InterlacePattern_ST.zw;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 interlace = tex2D(_InterlacePattern, i.uv2);

				c.rgb *= interlace.rgb;
				
				return c;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}