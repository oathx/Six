Shader "KingKong/Scene/BlendSpecUnlit"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
		_BlendMap("Gloss(R) Illum(G)", 2D) = "black" {}
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"RenderType" = "Opaque"
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			sampler2D _BlendMap;
			fixed _Shininess;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 spec : TEXCOORD1;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;

				fixed3 worldNormal = normalize(mul((fixed3x3)_Object2World, SCALED_NORMAL));
				fixed3 worldViewDir = normalize(WorldSpaceViewDir(v.vertex));
				fixed nv = max(0, dot(worldNormal, worldViewDir));
				o.spec = pow(nv, _Shininess * 128) * 2;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 b = tex2D(_BlendMap, i.uv);
				fixed4 color = c;

				color.rgb += i.spec * b.r;
				color.rgb += c.rgb * b.g;

				return color;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}