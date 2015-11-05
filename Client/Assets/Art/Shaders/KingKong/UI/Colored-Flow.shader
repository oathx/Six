Shader "KingKong/UI/Colored-Flow"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_FlowMap ("Flowmap (RGB)", 2D) = "black" {}
		_FlowSpeed ("FlowSpeed", Vector) = (1, 0, 0, 0)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest LEqual
			Offset -1, -1
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM	
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			sampler2D _FlowMap;
			fixed4 _FlowMap_ST;
			fixed4 _FlowSpeed;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord2 : TEXCOORD1;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed2 uv2 : TEXCOORD1;
				fixed4 color : COLOR;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				o.uv2 = v.texcoord2;
				o.color = v.color;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				fixed3 f = tex2D(_FlowMap, i.uv2);// +_FlowSpeed.xy * _Time.w);
				c *= i.color;
				c.rgb = f.rgb;// c.rgb * (1 - (fixed)f) + (fixed)f * f.rgb;

				return c;
			}

			ENDCG
		}
	}
}