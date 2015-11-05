// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "KingKong/Scene/BlendSpecLightmap"
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
#ifndef LIGHTMAP_OFF
			// sampler2D unity_Lightmap;
			fixed4 unity_LightmapST;
#endif

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
				fixed4 texcoord : TEXCOORD0;
#ifndef LIGHTMAP_OFF
				fixed4 texcoord1 : TEXCOORD1;
#endif
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 spec : TEXCOORD1;
#ifndef LIGHTMAP_OFF
				fixed2 lmap : TEXCOORD2;
#endif
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
#ifndef LIGHTMAP_OFF
				o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 b = tex2D(_BlendMap, i.uv);
				fixed4 color = c;

				color.rgb += i.spec * b.r;
#ifndef LIGHTMAP_OFF
				fixed3 l = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				color.rgb *= l;
#endif
				color.rgb += c.rgb * b.g;

				return color;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}