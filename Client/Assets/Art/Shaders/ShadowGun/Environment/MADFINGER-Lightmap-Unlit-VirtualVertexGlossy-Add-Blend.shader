// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "MADFINGER/Environment/Virtual Gloss Per-Vertex-Blend Additive (Supports Lightmap)"
{
	Properties
	{
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_SpecOffset ("Specular Offset from Camera", Vector) = (1, 10, 2, 0)
		_SpecRange ("Specular Range", Float) = 20
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ScrollingSpeed("Scrolling speed", Vector) = (0,0,0,0)
		_IllumColor ("Illumin Color", Color) = (1, 1, 1, 1)
		_MaskColor ("Mask Color", Color) = (1, 1, 1, 1)
		_BlendMap ("Blend Map [Spec(R) Illum(G) Mask(B)]", 2D) = "black" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
		LOD 100
		
		CGINCLUDE
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		fixed4 _MainTex_ST;
		
		#ifndef LIGHTMAP_OFF
		fixed4 unity_LightmapST;
		// sampler2D unity_Lightmap;
		#endif
		
		fixed3 _SpecOffset;
		fixed _SpecRange;
		fixed3 _SpecColor;
		fixed _Shininess;
		fixed4 _ScrollingSpeed;
		
		sampler2D _BlendMap;
		fixed3 _IllumColor;
		fixed3 _MaskColor;
		
		struct v2f
		{
			fixed4 pos : SV_POSITION;
			fixed2 uv : TEXCOORD0;
			#ifndef LIGHTMAP_OFF
			fixed2 lmap : TEXCOORD1;
			#endif
			fixed3 spec : TEXCOORD2;
		};

		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord + frac(_ScrollingSpeed * _Time.y);
			
			fixed3 viewNormal = mul((fixed3x3)UNITY_MATRIX_MV, v.normal);
			fixed4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);
			fixed3 viewDir = fixed3(0,0,1);
			fixed3 viewLightPos = _SpecOffset * fixed3(1,1,-1);
			fixed3 dirToLight = viewPos.xyz - viewLightPos;
			fixed3 h = (viewDir + normalize(-dirToLight)) * 0.5;
			fixed atten = 1.0 - saturate(length(dirToLight) / _SpecRange);

			o.spec = _SpecColor * pow(saturate(dot(viewNormal, normalize(h))), _Shininess * 128) * 2 * atten;
			
			#ifndef LIGHTMAP_OFF
			o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			#endif
			return o;
		}
		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 c = tex2D (_MainTex, i.uv);
				fixed3 spec = i.spec.rgb * c.a;
				fixed3 b = tex2D (_BlendMap, i.uv);
				
				fixed g = c.r * 0.11f + c.g * 0.59f + c.b * 0.3f;
				
				c.rgb = c.rgb * (1 - b.b) + g * _MaskColor.rgb * b.b;

				#if 1
				c.rgb += spec;
				#else
				c.rgb = c.rgb + spec - c.rgb * spec;
				#endif
				
				#ifndef LIGHTMAP_OFF
				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				c.rgb *= lm;
				#endif
				
				c.rgb += b.g * _IllumColor;

				return c;
			}
			ENDCG
		}
	}
}