Shader "KingKong/Effect/Dissolve"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
		_DissolveMap("Dissolvemap(Gray)", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range(0, 1)) = 0
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
			Fog{ Mode Off }
			Lighting Off
			ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			sampler2D _MainTex;
			sampler2D _DissolveMap;
			fixed4 _DissolveMap_ST;
			fixed _DissolveAmount;

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
				o.uv = v.texcoord.xy;

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed dissolve = tex2D(_DissolveMap, i.uv).r;
				clip(dissolve - _DissolveAmount);

				return c;
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}