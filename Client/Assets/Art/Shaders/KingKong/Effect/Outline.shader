Shader "KingKong/Effect/Outline"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlinePower("Outline Power", Range(0, 0.03)) = 0.008
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

			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode Off }
			Lighting Off
			Cull Off
			ZWrite Off

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			fixed4 _OutlineColor;
			fixed _OutlinePower;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				fixed3 normal = mul((fixed3x3)UNITY_MATRIX_IT_MV, v.normal);
				fixed3 offset = TransformViewToProjection(normal);
				o.pos.xy += offset.xy * o.pos.z * _OutlinePower;
				o.color = _OutlineColor;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}