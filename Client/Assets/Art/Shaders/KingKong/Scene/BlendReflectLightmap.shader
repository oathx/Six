// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "KingKong/Scene/BlendReflectLightmap"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
		_BlendMap("Reflect(R)", 2D) = "black" {}
		_Cube("Reflection Cubemap", CUBE) = "black" {}
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
			samplerCUBE _Cube;
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
				fixed3 refl : TEXCOORD1;
#ifndef LIGHTMAP_OFF
				fixed2 lmap : TEXCOORD2;
#endif
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;

				fixed3 worldNormal = mul((fixed3x3)_Object2World, v.normal);
				o.refl = reflect(-WorldSpaceViewDir(v.vertex), worldNormal);
#ifndef LIGHTMAP_OFF
				o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 b = tex2D(_BlendMap, i.uv);
				fixed3 f = texCUBE(_Cube, i.refl);

#ifndef LIGHTMAP_OFF
				fixed3 l = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				c.rgb *= l;
#endif
				c.rgb += f.rgb * b.r;

				return c;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}