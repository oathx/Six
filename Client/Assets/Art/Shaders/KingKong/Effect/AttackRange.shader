Shader "KingKong/Effect/AttackRange"
{
	Properties 
	{
		_Color("Main Color", Color) = (1, 1, 1, 1)
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
			Blend SrcAlpha One
			Fog{ Mode Off }
			Lighting Off
			ZWrite Off
			Offset -1, -1
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			fixed4 _Color;

			struct a2v
			{
				fixed4 vertex : POSITION;
			};

			struct v2f
			{
				fixed4 pos : SV_POSITION;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _Color;
			}

			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}